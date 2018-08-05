using System.ComponentModel;

namespace CommonCode.BusinessLayer
{
    public enum DataResultType
    {
        [Description("Unknown Error")]
        UnknownError = 0,

        [Description("Success")]
        Success,

        // States
        [Description("Unknown State")]
        UnknownState,

        [Description("Unsupported State")]
        UnsupportedState,

        [Description("Invalid State")]
        InvalidState,

        // Records
        [Description("Unknown Record")]
        UnknownRecord,

        [Description("Unable to Create Record")]
        UnableToCreateRecord,

        [Description("Unable to Read Record")]
        UnableToReadRecord,

        [Description("Unable to Update Record")]
        UnableToUpdateRecord,

        [Description("Unable to Delete Record")]
        UnableToDeleteRecord,

        // Permissions / Validation
        [Description("Confirmation Required")]
        ConfirmationRequired,

        [Description("Unauthorised")]
        Unauthorised,

        [Description("Validation Error")]
        ValidationError,

        // 'Soft' errors
        [Description("No Records Affected")]
        NoRecordsAffected,

        [Description("Not Required")]
        NotRequired,

        [Description("Old Version")]
        OldVersion,

        [Description("Disabled")]
        Disabled
    }
}