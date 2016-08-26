﻿// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JsonApiFramework.ServiceModel.Conventions;
using JsonApiFramework.ServiceModel.Internal;

namespace JsonApiFramework.ServiceModel.Configuration.Internal
{
    internal class AttributeInfoBuilder : IAttributeInfoBuilder
    {
        // PUBLIC CONSTRUCTORS //////////////////////////////////////////////
        #region Constructors
        public AttributeInfoBuilder(string clrPropertyName, Type clrPropertyType)
        {
            Contract.Requires(String.IsNullOrWhiteSpace(clrPropertyName) == false);
            Contract.Requires(clrPropertyType != null);

            var attributeInfoFactory = CreateAttributeInfoFactory(clrPropertyName, clrPropertyType);
            this.AttributeInfoFactory = attributeInfoFactory;
        }
        #endregion

        // PUBLIC METHODS ///////////////////////////////////////////////////
        #region IAttributeInfoBuilder Implementation
        public IAttributeInfoBuilder SetApiPropertyName(string apiPropertyName)
        {
            Contract.Requires(String.IsNullOrWhiteSpace(apiPropertyName) == false);

            this.AttributeInfoModifierCollection = this.AttributeInfoModifierCollection ?? new List<Action<AttributeInfo>>();
            this.AttributeInfoModifierCollection.Add(x => { x.ApiPropertyName = apiPropertyName; });
            return this;
        }
        #endregion

        // INTERNAL METHODS /////////////////////////////////////////////////
        #region Factory Methods
        internal IAttributeInfo CreateAttributeInfo(ConventionSet conventionSet)
        {
            var attributeInfo = this.AttributeInfoFactory(conventionSet);

            if (this.AttributeInfoModifierCollection == null)
                return attributeInfo;

            foreach (var attributeInfoModifier in this.AttributeInfoModifierCollection)
            {
                attributeInfoModifier(attributeInfo);
            }

            return attributeInfo;
        }
        #endregion

        // PRIVATE PROPERTIES ///////////////////////////////////////////////
        #region Properties
        private Func<ConventionSet, AttributeInfo> AttributeInfoFactory { get; set; }
        private IList<Action<AttributeInfo>> AttributeInfoModifierCollection { get; set; }
        #endregion

        // PRIVATE METHODS //////////////////////////////////////////////////
        #region Methods
        private static Func<ConventionSet, AttributeInfo> CreateAttributeInfoFactory(string clrPropertyName, Type clrPropertyType)
        {
            Contract.Requires(String.IsNullOrWhiteSpace(clrPropertyName) == false);
            Contract.Requires(clrPropertyType != null);

            Func<ConventionSet, AttributeInfo> attributeInfoFactory = (conventionSet) =>
                {
                    var apiPropertyName = clrPropertyName;
                    if (conventionSet != null && conventionSet.ApiAttributeNamingConventions != null)
                    {
                        apiPropertyName = conventionSet.ApiAttributeNamingConventions.Aggregate(apiPropertyName, (current, namingConvention) => namingConvention.Apply(current));
                    }

                    var attributeInfo = new AttributeInfo
                        {
                            // PropertyInfo Properties
                            ClrPropertyName = clrPropertyName,
                            ClrPropertyType = clrPropertyType,

                            // AttributeInfo Properties
                            ApiPropertyName = apiPropertyName,
                        };
                    return attributeInfo;
                };
            return attributeInfoFactory;
        }
        #endregion
    }
}