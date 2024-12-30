// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace SharpPusher.MVVM
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DependsOnPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DependsOnPropertyAttribute"/> using depending properties names.
        /// </summary>
        /// <param name="dependingPropertyNames">Names of the properties that the property with this attribute depends on.</param>
        public DependsOnPropertyAttribute(params string[] dependingPropertyNames)
        {
            DependentProps = dependingPropertyNames;
        }

        /// <summary>
        /// Array of all the properties that the property depends on!
        /// </summary>
        public readonly string[] DependentProps;
    }
}
