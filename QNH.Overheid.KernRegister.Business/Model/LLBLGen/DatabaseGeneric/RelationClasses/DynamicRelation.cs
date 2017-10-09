///////////////////////////////////////////////////////////////
// This is generated code. 
//////////////////////////////////////////////////////////////
// Code is generated using LLBLGen Pro version: 4.1
// Code is generated on: 
// Code is generated using templates: SD.TemplateBindings.SharedTemplates
// Templates vendor: Solutions Design.
//////////////////////////////////////////////////////////////
using System;
using KernregisterKvkData;
using KernregisterKvkData.FactoryClasses;
using KernregisterKvkData.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace KernregisterKvkData.RelationClasses
{
	/// <summary>Class to define dynamic relations for queries.</summary>
	/// <remarks>Dynamic relations are only supported in ansi joins so if you're using Oracle on 8i, you can't use Dynamic Relations. </remarks>
	[Serializable]
	public class DynamicRelation : DynamicRelationBase
	{
		/// <summary>Initializes a new instance of the <see cref="DynamicRelation"/> class.</summary>
		/// <param name="leftOperand">The left operand, which can only be a derived table definition. No join will take place. </param>
		/// <remarks>If a DynamicRelation is created with this CTor, it has to be the only one. It will be ignored if there are more
		/// relations in the relation collection.</remarks>
		public DynamicRelation(DerivedTableDefinition leftOperand)
		{
			this.InitClass(JoinHint.None, string.Empty, string.Empty, null, leftOperand, null);
		}
	
		/// <summary>Initializes a new instance of the <see cref="DynamicRelation"/> class.</summary>
		/// <param name="leftOperand">The left operand.</param>
		/// <param name="joinType">Type of the join. If None is specified, Inner is assumed.</param>
		/// <param name="rightOperand">The right operand.</param>
		/// <param name="onClause">The on clause for the join.</param>
		public DynamicRelation(DerivedTableDefinition leftOperand, JoinHint joinType, DerivedTableDefinition rightOperand, IPredicate onClause)
		{
			this.InitClass(joinType, string.Empty, string.Empty, onClause, leftOperand, rightOperand);
		}
	

		
		/// <summary>Gets the inheritance provider for inheritance info retrieval for entity operands</summary>
		/// <returns>The inheritance info provider</returns>
		protected override IInheritanceInfoProvider GetInheritanceProvider()
		{
			return InheritanceInfoProviderSingleton.GetInstance();
		}
	}
}