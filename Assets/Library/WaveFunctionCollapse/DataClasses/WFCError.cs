namespace FolvosLibrary.WFC
{
	public class ImpossibleDomainException : System.Exception
	{
		public ImpossibleDomainException() { }
		public ImpossibleDomainException(string message) : base(message) { }
		public ImpossibleDomainException(string message, System.Exception inner) : base(message, inner) { }
		protected ImpossibleDomainException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}