﻿/*
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
using System.Collections.Generic;

#endregion

namespace cloud.charging.apis.chargy
{

    public class Signature : ISignature
    {

        public String                   Algorithm        { get; }
        public SignatureFormats         Format           { get; }
        public String                   Value            { get; }
        public String                   PreviousValue    { get; }

        public Signature(String            Algorithm,
                         SignatureFormats  Format,
                         String            Value,
                         String            PreviousValue = null)
        {

            this.Algorithm      = Algorithm;
            this.Format         = Format;
            this.Value          = Value;
            this.PreviousValue = PreviousValue;

        }

        //public static implicit operator Signature(Signature2 Signature)

        //    => new Signature(Signature.Algorithm,
        //                     Signature.Format,
        //                     Signature.PreviousValue,
        //                     Signature.Value);

    }

    //public class Signature2 : ISignature2
    //{

    //    public String                   Algorithm        { get; }
    //    public SignatureFormats         Format           { get; }
    //    public String                   PreviousValue    { get; set; }
    //    public String                   Value            { get; set; }

    //           String                   ISignature.PreviousValue
    //        => this.PreviousValue;

    //           String                   ISignature.Value
    //        => this.Value;


    //    public Signature2(String            Algorithm,
    //                      SignatureFormats  Format,
    //                      String            PreviousValue,
    //                      String            Value)
    //    {

    //        this.Algorithm      = Algorithm;
    //        this.Format         = Format;
    //        this.PreviousValue  = PreviousValue;
    //        this.Value          = Value;

    //    }

    //}

}
