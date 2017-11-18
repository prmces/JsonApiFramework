﻿// Copyright (c) 2015–Present Scott McDonald. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.md in the project root for license information.

using System;
using System.Diagnostics.Contracts;

using JsonApiFramework.Reflection;

namespace JsonApiFramework.Metadata.Internal
{
    internal class AttributeInfo : IAttributeInfo
    {
        // PUBLIC CONSTRUCTORS //////////////////////////////////////////////
        #region Constructors
        public AttributeInfo(string apiAttributeName, string clrPropertyName, Type clrPropertyType, IClrPropertyBinding clrPropertyBinding)
        {
            Contract.Requires(String.IsNullOrWhiteSpace(apiAttributeName) == false);
            Contract.Requires(String.IsNullOrWhiteSpace(clrPropertyName) == false);
            Contract.Requires(clrPropertyType != null);
            Contract.Requires(clrPropertyBinding != null);

            this.ApiAttributeName = apiAttributeName;
            this.ClrPropertyName = clrPropertyName;
            this.ClrPropertyType = clrPropertyType;
            this.ClrPropertyBinding = clrPropertyBinding;

            // Calculate and set calculated properties.
            var isComplexType = GetIsComplexType(clrPropertyType);
            var isCollection = GetIsCollection(clrPropertyType, out var clrCollectionItemType);
            var isCollectionItemComplexType = isCollection ? GetIsComplexType(clrCollectionItemType) : default(bool?);

            this.IsComplexType = isComplexType;
            this.IsCollection = isCollection;
            this.ClrCollectionItemType = clrCollectionItemType;
            this.IsCollectionItemComplexType = isCollectionItemComplexType;
        }
        #endregion

        // PUBLIC PROPERTIES ////////////////////////////////////////////////
        #region IAttributeInfo Implementation
        public string ApiAttributeName { get; }
        public string ClrPropertyName { get; }
        public Type ClrPropertyType { get; }
        public bool IsComplexType { get; }
        public bool IsCollection { get; }
        public Type ClrCollectionItemType { get; }
        public bool? IsCollectionItemComplexType { get; }
        public IClrPropertyBinding ClrPropertyBinding { get; }
        #endregion

        // PRIVATE METHODS //////////////////////////////////////////////////
        #region Methods
        private static bool GetIsCollection(Type clrType, out Type collectionItemType)
        {
            if (clrType == null)
            {
                collectionItemType = null;
                return false;
            }

            var isCollection = TypeReflection.IsEnumerableOfT(clrType, out collectionItemType);
            return isCollection;
        }

        private static bool GetIsComplexType(Type clrType)
        {
            if (clrType == null)
                return false;

            var isComplexType = TypeReflection.IsComplex(clrType);
            return isComplexType;
        }
        #endregion
    }
}