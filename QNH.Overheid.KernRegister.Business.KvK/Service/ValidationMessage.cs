namespace QNH.Overheid.KernRegister.Business.Service
{
    public class ValidationMessage
    {

        public string Message { get; set; }
        public ValidationMessageType MessageType { get; set; }

        public ValidationMessage(string message, ValidationMessageType messageType)
        {
            Message = message;
            MessageType = messageType;
        }
    }
}