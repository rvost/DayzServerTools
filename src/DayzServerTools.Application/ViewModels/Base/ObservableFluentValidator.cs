using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation;
using FluentValidation.Results;

namespace DayzServerTools.Application.ViewModels.Base;

// Inspired by
// https://github.com/CommunityToolkit/dotnet/blob/main/CommunityToolkit.Mvvm/ComponentModel/ObservableValidator.cs
public class ObservableFluentValidator<TModel, TValidator> : ObservableObject, INotifyDataErrorInfo
    where TModel : class
    where TValidator : AbstractValidator<TModel>
{
    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="HasErrors"/>.
    /// </summary>
    private static readonly PropertyChangedEventArgs HasErrorsChangedEventArgs = new(nameof(HasErrors));

    protected readonly TModel _model;
    protected readonly TValidator _validator;
    protected int _totalErrors;

    public IDictionary<string, List<ValidationFailure>> Errors { get; }
    public TModel Model { get => _model; }
    public bool HasErrors => _totalErrors > 0;
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public ObservableFluentValidator(TModel model, TValidator validator)
    {
        _model = model;
        _validator = validator;

        Errors = new Dictionary<string, List<ValidationFailure>>();
    }

    public IEnumerable GetErrors(string propertyName = null)
    {
        // Get entity-level errors when the target property is null or empty
        if (string.IsNullOrEmpty(propertyName))
        {
            // Local function to gather all the entity-level errors
            [MethodImpl(MethodImplOptions.NoInlining)]
            IEnumerable<ValidationFailure> GetAllErrors()
            {
                return Errors.Values.SelectMany(static errors => errors);
            }

            return GetAllErrors();
        }

        // Property-level errors, if any
        if (Errors.TryGetValue(propertyName, out List<ValidationFailure> errors))
        {
            return errors;
        }

        // The INotifyDataErrorInfo.GetErrors method doesn't specify exactly what to
        // return when the input property name is invalid, but given that the return
        // type is marked as a non-nullable reference type, here we're returning an
        // empty array to respect the contract. This also matches the behavior of
        // this method whenever errors for a valid properties are retrieved.
        return Array.Empty<ValidationResult>();
    }
    IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName) => GetErrors(propertyName);

    public ValidationResult ValidateSelf()
    {
        var result = _validator.Validate(Model);

        if (_totalErrors != result.Errors.Count)
        {
            _totalErrors = result.Errors.Count;
            OnPropertyChanged(HasErrorsChangedEventArgs);
        }

        if (!result.IsValid)
        {
            var groupedErrors = result.Errors.GroupBy(
                e => e.PropertyName,
                (propertyName, errors) => (propertyName, errors)
                );

            foreach (var (propertyName, errors) in groupedErrors)
            {
                if (!Errors.TryGetValue(propertyName, out List<ValidationFailure> propertyErrors))
                {
                    propertyErrors = new List<ValidationFailure>();

                    Errors.Add(propertyName, propertyErrors);
                }

                bool errorsChanged = false;

                if (propertyErrors.Count > 0)
                {
                    propertyErrors.Clear();

                    errorsChanged = true;
                }

                propertyErrors.AddRange(errors);

                if (errorsChanged)
                {
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }
            }
        }

        return result;
    }

    protected bool SetProperty<T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, bool validate, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(propertyName);

        bool propertyChanged = SetProperty(oldValue, newValue, model, callback, propertyName);

        if (propertyChanged && validate)
        {
            ValidateProperty(propertyName);
        }

        return propertyChanged;
    }

    protected void ValidateProperty([CallerMemberName] string propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        if (!Errors.TryGetValue(propertyName, out List<ValidationFailure> propertyErrors))
        {
            propertyErrors = new List<ValidationFailure>();

            Errors.Add(propertyName, propertyErrors);
        }

        bool errorsChanged = false;

        if (propertyErrors.Count > 0)
        {
            propertyErrors.Clear();

            errorsChanged = true;
        }

        var result = _validator.Validate(Model, opt => opt.IncludeProperties(propertyName));
        propertyErrors.AddRange(result.Errors);

        // Update the shared counter for the number of errors, and raise the
        // property changed event if necessary. We decrement the number of total
        // errors if the current property is valid but it wasn't so before this
        // validation, and we increment it if the validation failed after being
        // correct before. The property changed event is raised whenever the
        // number of total errors is either decremented to 0, or incremented to 1.
        if (result.IsValid)
        {
            if (errorsChanged)
            {
                _totalErrors--;

                if (_totalErrors == 0)
                {
                    OnPropertyChanged(HasErrorsChangedEventArgs);
                }
            }
        }
        else if (!errorsChanged)
        {
            _totalErrors++;

            if (_totalErrors == 1)
            {
                OnPropertyChanged(HasErrorsChangedEventArgs);
            }
        }

        // Only raise the event once if needed. This happens either when the target property
        // had existing errors and is now valid, or if the validation has failed and there are
        // new errors to broadcast, regardless of the previous validation state for the property.
        if (errorsChanged || !result.IsValid)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
