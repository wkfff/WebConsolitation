using System;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Интерфейс фабрики
    /// </summary>
    internal abstract class EntityFactoryAbstract
    {
        internal abstract IEntity CreateEntity(string key, ServerSideObject owner, int ID, string semantic, string name,
                             ClassTypes classType, SubClassTypes subClassType, string configuration,
                             ServerSideObjectStates state); 
    }

    /// <summary>
    /// Фабрика для создания сущности
    /// </summary>
    internal class EntityFactoryBase : EntityFactoryAbstract
    {
        internal override IEntity CreateEntity(string key, ServerSideObject owner, int ID, string semantic, string name, ClassTypes classType, SubClassTypes subClassType, string configuration, ServerSideObjectStates state)
        {
            Entity entity;

            switch (classType)
            {
                case ClassTypes.clsFixedClassifier:
                    entity = new FixedClassifier(key, owner, semantic, name, state);
                    break;
                case ClassTypes.clsBridgeClassifier:
                    entity = new BridgeClassifier(key, owner, semantic, name, state);
                    break;
                case ClassTypes.clsDataClassifier:
                    entity = (string.Compare(semantic, VariantDataClassifier.VariantSemantic, true) == 0) ?
                        new VariantDataClassifier(key, owner, name, subClassType, state) :
                        new DataClassifier(key, owner, semantic, name, subClassType, state);
                    /*
                    if (semantic == VariantDataClassifier.VariantSemantic)
                        entity = new VariantDataClassifier(key, owner, name, subClassType, state);
                    else
                        entity = new DataClassifier(key, owner, semantic, name, subClassType, state);*/
                    break;
                case ClassTypes.clsFactData:
                    entity = new FactTable(key, owner, semantic, name, subClassType, state);
                    break;
                case ClassTypes.Table:
                    entity = new TableEntity(key, owner, semantic, name, state);
                    break;
				case ClassTypes.DocumentEntity:
					entity = new PresentationLayer.DocumentEntity(key, owner, semantic, name, state);
					break;
				default:
                    throw new Exception(String.Format("Объекты с классом {0} не поддерживаются.", classType));
            }
            entity.ID = ID;
            entity.Configuration = configuration;
            return entity;
        }
    }
}
