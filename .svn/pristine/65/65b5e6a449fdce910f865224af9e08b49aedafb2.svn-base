using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
	public class SmoAttributeDocumentEntity : SmoAttribute, IDocumentEntityAttribute
	{
		public SmoAttributeDocumentEntity(IDocumentEntityAttribute serverObject)
            : this(serverObject, false)
        {
        }

		public SmoAttributeDocumentEntity(IDocumentEntityAttribute serverObject, bool cached)
            : base(serverObject, cached)
        {
        }


		public SmoAttributeDocumentEntity(SMOSerializationInfo cache) 
            : base(cache)
        {
		}

		#region IDocumentEntityAttribute

		public string SourceEntityKey
		{
			get { return cached ? (string)GetCachedValue("SourceEntityKey") : ((IDocumentEntityAttribute)serverControl).SourceEntityKey; }
		}

		public string SourceEntityAttributeKey
		{
			get { return cached ? (string)GetCachedValue("SourceEntityAttributeKey") : ((IDocumentEntityAttribute)serverControl).SourceEntityAttributeKey; }
		}

		public void SetSourceAttribute(IDataAttribute sourceAttribute)
		{
			((IDocumentEntityAttribute) serverControl).SetSourceAttribute(sourceAttribute);
		}

		#endregion IDocumentEntityAttribute
	}
}
