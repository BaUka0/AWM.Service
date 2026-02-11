namespace AWM.Service.Domain.Errors;

public static class DomainErrors
{
    public static class Auth
    {
        public const string InvalidCredentials = $"{ErrorCodes.Unauthorized}.InvalidCredentials";
        public const string AccountDeactivated = $"{ErrorCodes.Unauthorized}.AccountDeactivated";
        public const string AccountDeleted = $"{ErrorCodes.Unauthorized}.AccountDeleted";
        public const string PasswordNotSet = $"{ErrorCodes.Unauthorized}.PasswordNotSet";

        public static class Registration
        {
            public const string UserAlreadyExists = $"{ErrorCodes.Validation}.User.AlreadyExists";
            public const string InvalidPassword = $"{ErrorCodes.Validation}.User.InvalidPassword";
            public const string InvalidEmail = $"{ErrorCodes.Validation}.User.InvalidEmail";
        }
    }

    public static class Org
    {
        public static class Institute
        {
            public const string NotFound = $"{ErrorCodes.NotFound}.Institute";
            public const string HasActiveDepartments = $"{ErrorCodes.BusinessRule}.Institute.HasActiveDepartments";
            public const string NameRequired = $"{ErrorCodes.Validation}.Institute.NameRequired";
            public const string GenericError = $"{ErrorCodes.Validation}.Institute";
        }

        public static class Department
        {
            public const string NotFound = $"{ErrorCodes.NotFound}.Department";
            public const string NameRequired = $"{ErrorCodes.Validation}.Department.NameRequired";
            public const string GenericError = $"{ErrorCodes.Validation}.Department";
            public const string HasActiveStaff = $"{ErrorCodes.BusinessRule}.Department.HasActiveStaff";
            public const string HasActivePrograms = $"{ErrorCodes.BusinessRule}.Department.HasActivePrograms";
            public const string HasActiveTopics = $"{ErrorCodes.BusinessRule}.Department.HasActiveTopics";
        }

        public static class University
        {
            public const string NotFound = $"{ErrorCodes.NotFound}.University";
        }
    }

    public static class General
    {
        public const string InternalError = ErrorCodes.InternalError;
    }
}
