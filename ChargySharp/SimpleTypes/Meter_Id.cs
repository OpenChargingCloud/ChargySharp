/*
 * Copyright (c) 2018-2019 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of ChargySharp <https://github.com/OpenChargingCloud/ChargySharp>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.apis.chargy
{

    /// <summary>
    /// The unique identification of a meter.
    /// </summary>
    public struct Meter_Id : IId,
                             IEquatable <Meter_Id>,
                             IComparable<Meter_Id>

    {

        #region Data

        private readonly static Random _Random = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the meter identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter identification.
        /// based on the given string.
        /// </summary>
        /// <param name="Text">The text representation of a meter identification.</param>
        private Meter_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a meter identification.
        /// </summary>
        /// <param name="Text">A text representation of a meter identification.</param>
        public static Meter_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a meter identification must not be null or empty!");

            #endregion

            if (TryParse(Text, out Meter_Id EnergyMeterId))
                return EnergyMeterId;

            throw new ArgumentNullException(nameof(Text), "The given text representation of a meter identification is invalid!");

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a meter identification.
        /// </summary>
        /// <param name="Text">A text representation of a meter identification.</param>
        public static Meter_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Meter_Id EnergyMeterId))
                return EnergyMeterId;

            return new Meter_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out EnergyMeterId)

        /// <summary>
        /// Try to parse the given string as a meter identification.
        /// </summary>
        /// <param name="Text">A text representation of a meter identification.</param>
        /// <param name="MeterId">The parsed meter identification.</param>
        public static Boolean TryParse(String Text, out Meter_Id MeterId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                MeterId = default;
                return false;
            }

            #endregion

            try
            {
                MeterId = new Meter_Id(Text);
                return true;
            }
            catch (Exception)
            { }

            MeterId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this meter identification.
        /// </summary>
        public Meter_Id Clone

            => new Meter_Id(
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Provider == (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">A meter identification.</param>
        /// <param name="EnergyMeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Meter_Id EnergyMeterId1, Meter_Id EnergyMeterId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(EnergyMeterId1, EnergyMeterId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) EnergyMeterId1 == null) || ((Object) EnergyMeterId2 == null))
                return false;

            return EnergyMeterId1.Equals(EnergyMeterId2);

        }

        #endregion

        #region Provider != (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">A meter identification.</param>
        /// <param name="EnergyMeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Meter_Id EnergyMeterId1, Meter_Id EnergyMeterId2)
            => !(EnergyMeterId1 == EnergyMeterId2);

        #endregion

        #region Provider <  (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">A meter identification.</param>
        /// <param name="EnergyMeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Meter_Id EnergyMeterId1, Meter_Id EnergyMeterId2)
        {

            if ((Object) EnergyMeterId1 == null)
                throw new ArgumentNullException(nameof(EnergyMeterId1), "The given EnergyMeterId1 must not be null!");

            return EnergyMeterId1.CompareTo(EnergyMeterId2) < 0;

        }

        #endregion

        #region Provider <= (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">A meter identification.</param>
        /// <param name="EnergyMeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Meter_Id EnergyMeterId1, Meter_Id EnergyMeterId2)
            => !(EnergyMeterId1 > EnergyMeterId2);

        #endregion

        #region Provider >  (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">A meter identification.</param>
        /// <param name="EnergyMeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Meter_Id EnergyMeterId1, Meter_Id EnergyMeterId2)
        {

            if ((Object) EnergyMeterId1 == null)
                throw new ArgumentNullException(nameof(EnergyMeterId1), "The given EnergyMeterId1 must not be null!");

            return EnergyMeterId1.CompareTo(EnergyMeterId2) > 0;

        }

        #endregion

        #region Provider >= (EnergyMeterId1, EnergyMeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId1">A meter identification.</param>
        /// <param name="EnergyMeterId2">Another meter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Meter_Id EnergyMeterId1, Meter_Id EnergyMeterId2)
            => !(EnergyMeterId1 < EnergyMeterId2);

        #endregion

        #endregion

        #region IComparable<EnergyMeterId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is Meter_Id EnergyMeterId))
                throw new ArgumentException("The given object is not a meter identification!",
                                            nameof(Object));

            return CompareTo(EnergyMeterId);

        }

        #endregion

        #region CompareTo(EnergyMeterId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EnergyMeterId">An object to compare with.</param>
        public Int32 CompareTo(Meter_Id EnergyMeterId)
        {

            if ((Object) EnergyMeterId == null)
                throw new ArgumentNullException(nameof(EnergyMeterId),  "The given meter identification must not be null!");

            return String.Compare(InternalId, EnergyMeterId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region IEquatable<EnergyMeterId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            if (!(Object is Meter_Id EnergyMeterId))
                return false;

            return Equals(EnergyMeterId);

        }

        #endregion

        #region Equals(EnergyMeterId)

        /// <summary>
        /// Compares two EnergyMeterIds for equality.
        /// </summary>
        /// <param name="EnergyMeterId">A meter identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Meter_Id EnergyMeterId)
        {

            if ((Object) EnergyMeterId == null)
                return false;

            return InternalId.ToLower().Equals(EnergyMeterId.InternalId.ToLower());

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => InternalId.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
