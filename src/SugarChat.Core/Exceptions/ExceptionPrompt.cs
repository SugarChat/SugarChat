using SugarChat.Core.Common;

namespace SugarChat.Core.Exceptions
{
    public record ExceptionPrompt
    {
        private readonly string _formatString;
        private string[] _contents = {};

        public ExceptionPrompt(ExceptionCode code, string formatString)
        {
            _formatString = formatString;
            Code = (int)code;
        }

        public int Code { get; }
        public string Message => string.Format(_formatString, _contents);

        public ExceptionPrompt WithParams(params string[] contents)
        {
            _contents = contents;
            return this;
        }
    }
}