using System;
using System.Text.RegularExpressions;

namespace ProviderQuality.Console
{
    /// <summary>
    /// A class for modeling awards with a quality score and expiration date. Award quality can be used as a measure of gauging service provided
    /// </summary>
    public class Award
    { 
        /// <summary>
        /// The different kinds of awards that we can have. 
        /// Used enums to eliminate magic strings and put the logic of naming the awards into the Award class itself. 
        /// </summary>
        public enum AwardType
        {
            Gov_Quality_Plus,
            Blue_First, 
            Acme_Partner_Facility,
            Blue_Distinction_Plus,
            Blue_Compare,
            Top_Connected_Providers,
            Blue_Star
            //New Award Types Go Here. 
        }

        /// <summary>
        /// The type of this instance of award. Used for update and set up logic.
        /// </summary>
        private AwardType thisAwardType;

        /// <summary>
        /// The name of this award type. The name is assigned on instantiation as using the selected Award Type Enum. 
        /// </summary>
        private string name; 

        /// <summary>
        /// The days until this award expires. Additional update logic is added after the award expires. 
        /// </summary>
        private int expiresIn;

        /// <summary>
        /// The quality value of this award used for comparison between award recipients. 
        /// </summary>
        private int quality; 

        /// <summary>
        /// The decrementation factor of this award. It is added to the ward each day until the quality reaches a cap of 0 or 50.
        /// The post expiration decrementation factor is added to our quality calculations after the ExpiresIn value has reached 0. 
        /// This defaults to -1 and -1 because 3 of our seven existing awards decrement by 1 each step. 
        /// </summary>
        private int decrementFactor = -1,
            postExpirationDecrementFactor = -1; 

        /// <summary>
        /// The constructor to set up this award type when the instance is created. 
        /// </summary>
        /// <param name="type"> The type of award to be created based on the enum list of available award types. </param>
        public Award(AwardType type)
        {
            thisAwardType = type;
            SetUp(); 
        }


        /// <summary>
        /// Update the quality and expiration date of this award. 
        /// </summary>
        public void UpdateQuality()
        {
            //If the award is Blue distinction plus we do no work. 
            if (thisAwardType == AwardType.Blue_Distinction_Plus)
            {
                return;
            }

            //Decrement expires in by 1. 
            if (ExpiresIn > 0)
            {
                expiresIn -= 1;
            }

            //The conditional logic for the different award types. 
            switch (thisAwardType)
            {
                case AwardType.Gov_Quality_Plus:
                case AwardType.Acme_Partner_Facility:
                case AwardType.Top_Connected_Providers:
                case AwardType.Blue_Star:
                    //The four award types above simply decrement with each passing day towards expiration.
                    if (quality > 0)
                    {
                        quality += decrementFactor;
                    }
                    if (expiresIn <= 0 && quality > 0)
                    {
                        //If we have expired but still have quality value then add on the post expiration decrementation factor to double the speed at which quality decreases. 
                        quality += postExpirationDecrementFactor;
                    }
                    break;
                case AwardType.Blue_Compare:
                    //If we we have expired but still have quality
                    if(expiresIn <= 0)
                    {
                        quality = 0;
                        break;
                    }
                    else if(quality < 50)
                    {
                        //Add the decrement factor
                        quality += decrementFactor;
                        if(expiresIn < 11 && quality < 50)
                        {
                            //If we have less than 11 days to expiration add decrement factor again. 
                            quality += decrementFactor; 
                        }

                        if(expiresIn < 6 && quality < 50)
                        {
                            //If we have less than 6 days add decrement factor again. 
                            quality += decrementFactor; 
                        }
                    }
                    break; 
                case AwardType.Blue_First:
                    if(quality < 50)
                    {
                        //Add decrement factor until we reach quality score of 50. 
                        quality += decrementFactor; 
                    }
                    break;
                //New Award Type update logic goes here. 
                default:
                    throw new ArgumentException("Unknown Enum Value Type. Are you sure the award type you are casting to is valid?");
            }

            CheckValues(); 
        }

        /// <summary>
        /// Set up the initial values that will be used during the update step of this award. 
        /// </summary>
        private void SetUp()
        {
            //Set the name property based on the enum value that we have selected. 
            SetName();

            switch (thisAwardType)
            {
                case AwardType.Gov_Quality_Plus:
                    //uses default decrement and post decrement factors. 
                    expiresIn = 10;
                    quality = 20;
                    break;
                case AwardType.Blue_First:
                    //uses positive decrement factors to increase quality score over time. Has no post decrement.  
                    expiresIn = 2;
                    quality = 0;
                    decrementFactor = 1;
                    postExpirationDecrementFactor = 0;
                    break;
                case AwardType.Acme_Partner_Facility:
                    //uses default decrement and post decrement factors. 
                    expiresIn = 5;
                    quality = 7;
                    break;
                case AwardType.Blue_Distinction_Plus:
                    //Cannot expire, has highest quality, and quality does not increase or decrease. 
                    expiresIn = 0;
                    quality = 80;
                    decrementFactor = 0;
                    postExpirationDecrementFactor = 0;
                    break;
                case AwardType.Blue_Compare:
                    //uses a positive decrement factor to increase quality. After it expires quality drops to 0. 
                    expiresIn = 15;
                    quality = 20;
                    decrementFactor = 1;
                    postExpirationDecrementFactor = 0;
                    break;
                case AwardType.Top_Connected_Providers:
                    //uses default decrement and post decrement factors. 
                    expiresIn = 3;
                    quality = 6;
                    break;
                case AwardType.Blue_Star:
                    //Expires at double the rate of the other default awards, so both the decrement and post expiration decrement factors are doubled. 
                    expiresIn = 10;
                    quality = 30;
                    decrementFactor *= 2;
                    postExpirationDecrementFactor *= 2;
                    break;
                //New Award Type Instantiation Set Up Logic Goes Here. 
                default:
                    throw new ArgumentException("Unknown Enum Value Type. Are you sure the award type you are casting to is valid?");
            }
        }

        /// <summary>
        /// Turn the name of this award into the selected award type enum, cast as a string. 
        /// </summary>
        private void SetName()
        {
            string myName = thisAwardType.ToString();
            //Remove non letters and numbers from the enum name, turning those characters into a white space. 
            myName = Regex.Replace(myName, @"[^A-Za-z0-9]+", " ");
            name = myName;
        }

        /// <summary>
        /// Make sure our quality value exists between a range of 0-50 for all types but Blue Distinction Plus. 
        /// Also make sure our ExpiresIn  value never becomes negative. 
        /// </summary>
        private void CheckValues()
        {
            if (quality > 50 && thisAwardType != AwardType.Blue_Distinction_Plus)
            {
                quality = 50;
            }

            if (quality < 0)
            {
                quality = 0;
            }

            if (expiresIn < 0)
            {
                expiresIn = 0;
            }
        }

        //The getter properties for accessing the relevant fields outside of the Award Class.
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public int ExpiresIn
        {
            get
            {
                return this.expiresIn;
            }
        }

        public int Quality
        {
            get
            {
                return this.quality;
            }
        }
    }
}
