using System;
using System.Collections.Generic;
using System.Globalization;

namespace DotSpatial.SDR.Plugins.Search
{
    public class StreetAddress
    {
        public string Number { get; set; }
        public string Predirectional { get; set; }
        public string PreType { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string Postdirectional { get; set; }
        public string SubUnitType { get; set; }
        public string SubUnitValue { get; set; }
    }

    public class StreetAddressParser
    {
        // words to search for on streetnames looking for pretypes
        private static readonly string[] PreTypes = {"HIGHWAY", "HWY", "HIWAY", "HIWY", "HWAY", "ROAD", "RD"};

        public static StreetAddress Parse(string address)
        {
            if (string.IsNullOrEmpty(address))  // make sure there is a query to start with
                return new StreetAddress();

            // parse our query into individual string chunks
            string[] input = address.ToUpper().Split(' ');
            // ensure we have an input to begin with
            if (input.Length > 0)
            {
                string numberPos = null;
                string predirPos = null;
                string strNamePos = null;
                string strTypePos = null;
                string postdirPos = null;
                string subtypePos = null;
                string subvalPos = null;

                var last = input.Length - 1;  // the last poistion available for parsing
                var result = new StreetAddress();

                // first lets see if the last word is postdirectional value
                if (Directionals.ContainsValue(input[last]) || Directionals.ContainsKey(input[last]))
                {
                    // we have a valid postdirectional set as the last word
                    postdirPos = last.ToString(CultureInfo.InvariantCulture);
                    // only continue the check if there is more than one word present
                    if (last - 1 > 0)
                    {
                        // check if the previous word is a valid street type
                        if (Suffixes.ContainsValue(input[last - 1]) || Suffixes.ContainsKey(input[last - 1]))
                        {
                            // the second to last word is a valid street type
                            strTypePos = (last - 1).ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
                // last word is not a directional lets go ahead and search for a valid road type
                else if (Suffixes.ContainsValue(input[last]) || Suffixes.ContainsKey(input[last]))
                {
                    // the last word is a valid road type
                    strTypePos = last.ToString(CultureInfo.InvariantCulture);
                }
                // well the last word is not a directional or valid road type so lets look for a subunit
                else if (UnrangedSubunits.ContainsValue(input[last]) || UnrangedSubunits.ContainsKey(input[last]))
                {
                    // a valid subunit was found that has no associated value
                    subtypePos = last.ToString(CultureInfo.InvariantCulture);
                    // continue if there is more than a single word present in the query
                    if (last - 1 > 0)
                    {
                        // check if there is a post directional before this word
                        if (Directionals.ContainsValue(input[last - 1]) || Directionals.ContainsKey(input[last - 1]))
                        {
                            // postdirectional was found right before the subtype word
                            postdirPos = (last - 1).ToString(CultureInfo.InvariantCulture);
                            // now lets look if there is a valid street type before the directional
                            if (Suffixes.ContainsValue(input[last - 2]) || Suffixes.ContainsKey(input[last - 2]))
                            {
                                // there is a valid street type right before the directional
                                strTypePos = (last - 2).ToString(CultureInfo.InvariantCulture);
                            }
                        }
                        // no post directional was found in the preceding word lets go ahead and look for a valid street type instead
                        else if (Suffixes.ContainsValue(input[last - 1]) || Suffixes.ContainsKey(input[last - 1]))
                        {
                            // we have a valid street type set previous to the subunit type
                            strTypePos = (last - 1).ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
                // this is more than likely a ranged_subunit value lets look at the previous word and verify it is a type
                else
                {
                    if (last - 1 > 0)
                    {
                        if (RangedSubunits.ContainsValue(input[last - 1]) ||
                            RangedSubunits.ContainsKey(input[last - 1]))
                        {
                            // yes this is a valid subtype lets make the previous value its subunit value
                            subtypePos = (last - 1).ToString(CultureInfo.InvariantCulture);
                            subvalPos = last.ToString(CultureInfo.InvariantCulture);

                            if (last - 2 > 0)  // validate we have enough words
                            {
                                // now lets look if the words previous to the subunit type are directional
                                if (Directionals.ContainsValue(input[last - 2]) || Directionals.ContainsKey(input[last - 2]))
                                {
                                    // looks like the word previous to subunit type is a directional
                                    postdirPos = (last - 2).ToString(CultureInfo.InvariantCulture);

                                    if (last - 3 > 0)  // validate that enough words exist to continue
                                    {
                                        // now lets look and see if the word previous to that is a valid road type
                                        if (Suffixes.ContainsValue(input[last - 3]) || Suffixes.ContainsKey(input[last - 3]))
                                        {
                                            // ok cool so it looks like the previous word is a valid road type
                                            strTypePos = (last - 3).ToString(CultureInfo.InvariantCulture);
                                        }
                                    }
                                }                            
                                // well its not a directional so lets check for a valid road type instead
                                else if (Suffixes.ContainsValue(input[last - 2]) || Suffixes.ContainsKey(input[last - 2]))
                                {
                                    // yup we have a valid road type set it now
                                    strTypePos = (last - 2).ToString(CultureInfo.InvariantCulture);
                                }
                            }
                        }
                    }
                }
                // ok that covers all the end of string stuff lets look at the start of the string now
                try
                {
                    // see if the first word is an int (attempt to cast -> try/catch)
                    Convert.ToInt32(input[0]);
                    numberPos = "0";  // we know the first position is a structure number
                    if (input.Length > 1)  // make sure that another word exists in the input
                    {
                        // check the next word is a unranged subtype
                        if (UnrangedSubunits.ContainsValue(input[1]) || UnrangedSubunits.ContainsKey(input[1]))
                        {
                            // this is a valid subtype
                            subtypePos = "1";
                            // again validate enough length
                            if (input.Length > 2)
                            {
                                // check the next word is a directional
                                if (Directionals.ContainsValue(input[2]) || Directionals.ContainsKey(input[2]))
                                {
                                    // ok so we have a pre directional immediately after the subunit
                                    predirPos = "2";
                                    // a final check for array length criteria
                                    if (input.Length > 3)
                                    {
                                        // as such we can safely assume the next word is the start of the road name
                                        strNamePos = "3";
                                    }
                                }
                                // no directional after the subunit so this is the start of the road name
                                else
                                {
                                    strNamePos = "2";
                                }
                            }
                        }
                        // check the next word is a ranged subtype
                        else if (RangedSubunits.ContainsValue(input[1]) || RangedSubunits.ContainsKey(input[1]))
                        {
                            // this is a valid subtype
                            subtypePos = "1";
                            // as such the next value will be a value for the subtype
                            if (input.Length > 2)
                            {
                                subvalPos = "2";
                            }
                            if (input.Length > 3)
                            {
                                // check the next word is a directional
                                if (Directionals.ContainsValue(input[3]) || Directionals.ContainsKey(input[3]))
                                {
                                    // ok so we have a pre directional immediately after the subunit value
                                    predirPos = "3";
                                    if (input.Length > 4)
                                    {
                                        // as such we can safely assume the next word is the start of the road name
                                        strNamePos = "4";
                                    }
                                }
                                // no directional after the subunit so this is the start of the road name
                                else
                                {
                                    strNamePos = "3";
                                }
                            }
                        }
                        // check if the next word is a pre directional
                        else if (Directionals.ContainsValue(input[1]) || Directionals.ContainsKey(input[1]))
                        {
                            // so the next word is a predirectional means the next word after it will be the road_name_start
                            predirPos = "1";
                            if (input.Length > 2)
                            {
                                strNamePos = "2";
                            }
                        }
                        // turns out the next word is the actual start of the road name
                        else
                        {
                            strNamePos = "1";
                        }
                    }
                }
                catch
                {
                    // failed to cast / first word is not a number so lets look for a predirectional first
                    if (Directionals.ContainsValue(input[0]) || Directionals.ContainsKey(input[0]))
                    {
                        // seems like the first word is a directional we can also assume the road name starts after the directional
                        predirPos = "0";
                        if (input.Length > 1)
                        {
                            strNamePos = "1";
                        }
                    } else {
                        // not a directional so we can assume the first word is actually the road name start
                        strNamePos = "0";
                    }
                }
                // check if we have a value set for the start of the road name
                string strNameEnd;
                if (strNamePos == strTypePos)
                {
                    if (predirPos != null)
                    {
                        // looks like the predirectional is actually the road name. ie west lane etc.
                        strNamePos = predirPos;
                        predirPos = null;
                    }
                }
                // lets figure out where the end of the road name string is
                if (strTypePos != null && Convert.ToInt32(strTypePos) > Convert.ToInt32(strNamePos))
                {
                    // we have a road type which means the previous word is the end of the road name
                    strNameEnd = (Convert.ToInt32(strTypePos) - 1).ToString(CultureInfo.InvariantCulture);
                }
                // well doesnt look like we have a road type so lets look for a postdirectional instead
                else if (postdirPos != null && Convert.ToInt32(postdirPos) > Convert.ToInt32(strNamePos))
                {
                    // we have a post directional and no road type we can assume the previous word is the end of road name
                    strNameEnd = (Convert.ToInt32(postdirPos) - 1).ToString(CultureInfo.InvariantCulture);
                }
                // ok now we have no road type and no post directional lets look for a subunit
                else if (subtypePos != null && Convert.ToInt32(subtypePos) > Convert.ToInt32(strNamePos))
                {
                    // we have a subunit so we can assume the previous word is the end of road name
                    strNameEnd = (Convert.ToInt32(subtypePos) - 1).ToString(CultureInfo.InvariantCulture);
                }
                // no road type, no postdirectional, no subtype, we shouldnt ever get here but lets look for a subvalue
                else if (subvalPos != null && Convert.ToInt32(subvalPos) > Convert.ToInt32(strNamePos))
                {
                    // somehow we have a subvalue and no subtype, we can assume the previous word is end of road name and FUCKED
                    strNameEnd = (Convert.ToInt32(subvalPos) - 1).ToString(CultureInfo.InvariantCulture);
                }
                // there are no positions beyond the end of the string name
                else
                {
                    strNameEnd = Convert.ToInt32(last).ToString(CultureInfo.InvariantCulture);
                }
                // now lets go ahead and assign all this shit out
                if (numberPos != null) 
                {
                    result.Number = input[Convert.ToInt32(numberPos)];
                }
                if (subtypePos != null)
                {
                    result.SubUnitType =  input[Convert.ToInt32(subtypePos)];
                }
                if (subvalPos != null)
                {
                    result.SubUnitValue =  input[Convert.ToInt32(subvalPos)];
                }
                if (predirPos != null)
                {
                    result.Predirectional = input[Convert.ToInt32(predirPos)];
                }
                if (strTypePos != null)
                {
                    result.StreetType = input[Convert.ToInt32(strTypePos)];
                }
                if (postdirPos != null)
                {
                    result.Postdirectional = input[Convert.ToInt32(postdirPos)];
                }
                if (strNamePos == null) return result;  // no street name return it as is

                // assemble the temp streetname value now
                var strNameTmp = string.Empty;
                for (var i = Convert.ToInt32(strNamePos); i <= Convert.ToInt32(strNameEnd); i++)
                {
                    strNameTmp = strNameTmp + input[i] + " ";
                }
                // break this apart once more to search for pretype values
                string[] strNameArr = strNameTmp.ToUpper().Split(' ');
                // check for pretypes within the street name
                if (strNameArr.Length >= 2)
                {
                    switch (strNameArr[0])
                    {
                        case "INTERSTATE":
                            // assign pretype and street name
                            result.PreType = strNameArr[0];
                            result.StreetName = strNameTmp.Replace(strNameArr[0], "").Trim();
                            break;
                        case "COUNTY":
                        case "STATE":
                        case "US":
                            // check if the second word is one of the pretypes (HIGHWAY, ROAD, and abrv's)
                            var exists = Array.Exists(PreTypes, s => s.Equals(strNameArr[1]));
                            if (exists)
                            {
                                // assign pretype and street name
                                var pretype = strNameArr[0] + " " + strNameArr[1];
                                result.PreType = pretype;
                                result.StreetName = strNameTmp.Replace(pretype, "").Trim();
                            }
                            else // no pretype set the whole thing as streetname
                            {
                                result.StreetName = strNameTmp.Trim();
                            }
                            break;
                        default:  // no pretype again set the whole thing as streetname
                            result.StreetName = strNameTmp.Trim();
                            break;
                    }
                }
                else // no pretype is possible assign the streetname
                {
                    result.StreetName = strNameTmp.Trim();
                }
                return result;
            }
            return null;
        }

        private static readonly Dictionary<string, string> Directionals =
            new Dictionary<string, string>
            {
                { "NORTH", "N" },
                { "NORTHEAST", "NE" },
                { "EAST", "E" },
                { "SOUTHEAST", "SE" },
                { "SOUTH", "S" },
                { "SOUTHWEST", "SW" },
                { "WEST", "W" },
                { "NORTHWEST", "NW" }
            };

        private static readonly Dictionary<string, string> Suffixes =
            new Dictionary<string, string>
            {
                { "ALLEE", "ALY" },
                { "ALLEY", "ALY" },
                { "ALLY", "ALY" },
                { "ANEX", "ANX" },
                { "ANNEX", "ANX" },
                { "ANNX", "ANX" },
                { "ARCADE", "ARC" },
                { "AV", "AVE" },
                { "AVEN", "AVE" },
                { "AVENU", "AVE" },
                { "AVENUE", "AVE" },
                { "AVN", "AVE" },
                { "AVNUE", "AVE" },
                { "BAYOO", "BYU" },
                { "BAYOU", "BYU" },
                { "BEACH", "BCH" },
                { "BEND", "BND" },
                { "BLUF", "BLF" },
                { "BLUFF", "BLF" },
                { "BLUFFS", "BLFS" },
                { "BOT", "BTM" },
                { "BOTTM", "BTM" },
                { "BOTTOM", "BTM" },
                { "BOUL", "BLVD" },
                { "BOULEVARD", "BLVD" },
                { "BOULV", "BLVD" },
                { "BRANCH", "BR" },
                { "BRDGE", "BRG" },
                { "BRIDGE", "BRG" },
                { "BRNCH", "BR" },
                { "BROOK", "BRK" },
                { "BROOKS", "BRKS" },
                { "BURG", "BG" },
                { "BURGS", "BGS" },
                { "BYPA", "BYP" },
                { "BYPAS", "BYP" },
                { "BYPASS", "BYP" },
                { "BYPS", "BYP" },
                { "CAMP", "CP" },
                { "CANYN", "CYN" },
                { "CANYON", "CYN" },
                { "CAPE", "CPE" },
                { "CAUSEWAY", "CSWY" },
                { "CAUSWAY", "CSWY" },
                { "CEN", "CTR" },
                { "CENT", "CTR" },
                { "CENTER", "CTR" },
                { "CENTERS", "CTRS" },
                { "CENTR", "CTR" },
                { "CENTRE", "CTR" },
                { "CIRC", "CIR" },
                { "CIRCL", "CIR" },
                { "CIRCLE", "CIR" },
                { "CIRCLES", "CIRS" },
                { "CK", "CRK" },
                { "CLIFF", "CLF" },
                { "CLIFFS", "CLFS" },
                { "CLUB", "CLB" },
                { "CMP", "CP" },
                { "CNTER", "CTR" },
                { "CNTR", "CTR" },
                { "CNYN", "CYN" },
                { "COMMON", "CMN" },
                { "CORNER", "COR" },
                { "CORNERS", "CORS" },
                { "COURSE", "CRSE" },
                { "COURT", "CT" },
                { "COURTS", "CTS" },
                { "COVE", "CV" },
                { "COVES", "CVS" },
                { "CR", "CRK" },
                { "CRCL", "CIR" },
                { "CRCLE", "CIR" },
                { "CRECENT", "CRES" },
                { "CREEK", "CRK" },
                { "CRESCENT", "CRES" },
                { "CRESENT", "CRES" },
                { "CREST", "CRST" },
                { "CROSSING", "XING" },
                { "CROSSROAD", "XRD" },
                { "CRSCNT", "CRES" },
                { "CRSENT", "CRES" },
                { "CRSNT", "CRES" },
                { "CRSSING", "XING" },
                { "CRSSNG", "XING" },
                { "CRT", "CT" },
                { "CURVE", "CURV" },
                { "DALE", "DL" },
                { "DAM", "DM" },
                { "DIV", "DV" },
                { "DIVIDE", "DV" },
                { "DRIV", "DR" },
                { "DRIVE", "DR" },
                { "DRIVES", "DRS" },
                { "DRV", "DR" },
                { "DVD", "DV" },
                { "ESTATE", "EST" },
                { "ESTATES", "ESTS" },
                { "EXP", "EXPY" },
                { "EXPR", "EXPY" },
                { "EXPRESS", "EXPY" },
                { "EXPRESSWAY", "EXPY" },
                { "EXPW", "EXPY" },
                { "EXTENSION", "EXT" },
                { "EXTENSIONS", "EXTS" },
                { "EXTN", "EXT" },
                { "EXTNSN", "EXT" },
                { "FALLS", "FLS" },
                { "FERRY", "FRY" },
                { "FIELD", "FLD" },
                { "FIELDS", "FLDS" },
                { "FLAT", "FLT" },
                { "FLATS", "FLTS" },
                { "FORD", "FRD" },
                { "FORDS", "FRDS" },
                { "FOREST", "FRST" },
                { "FORESTS", "FRST" },
                { "FORG", "FRG" },
                { "FORGE", "FRG" },
                { "FORGES", "FRGS" },
                { "FORK", "FRK" },
                { "FORKS", "FRKS" },
                { "FORT", "FT" },
                { "FREEWAY", "FWY" },
                { "FREEWY", "FWY" },
                { "FRRY", "FRY" },
                { "FRT", "FT" },
                { "FRWAY", "FWY" },
                { "FRWY", "FWY" },
                { "GARDEN", "GDN" },
                { "GARDENS", "GDNS" },
                { "GARDN", "GDN" },
                { "GATEWAY", "GTWY" },
                { "GATEWY", "GTWY" },
                { "GATWAY", "GTWY" },
                { "GLEN", "GLN" },
                { "GLENS", "GLNS" },
                { "GRDEN", "GDN" },
                { "GRDN", "GDN" },
                { "GRDNS", "GDNS" },
                { "GREEN", "GRN" },
                { "GREENS", "GRNS" },
                { "GROV", "GRV" },
                { "GROVE", "GRV" },
                { "GROVES", "GRVS" },
                { "GTWAY", "GTWY" },
                { "HARB", "HBR" },
                { "HARBOR", "HBR" },
                { "HARBORS", "HBRS" },
                { "HARBR", "HBR" },
                { "HAVEN", "HVN" },
                { "HAVN", "HVN" },
                { "HEIGHT", "HTS" },
                { "HEIGHTS", "HTS" },
                { "HGTS", "HTS" },
                { "HIGHWAY", "HWY" },
                { "HIGHWY", "HWY" },
                { "HILL", "HL" },
                { "HILLS", "HLS" },
                { "HIWAY", "HWY" },
                { "HIWY", "HWY" },
                { "HLLW", "HOLW" },
                { "HOLLOW", "HOLW" },
                { "HOLLOWS", "HOLW" },
                { "HOLWS", "HOLW" },
                { "HRBOR", "HBR" },
                { "HT", "HTS" },
                { "HWAY", "HWY" },
                { "INLET", "INLT" },
                { "ISLAND", "IS" },
                { "ISLANDS", "ISS" },
                { "ISLES", "ISLE" },
                { "ISLND", "IS" },
                { "ISLNDS", "ISS" },
                { "JCTION", "JCT" },
                { "JCTN", "JCT" },
                { "JCTNS", "JCTS" },
                { "JUNCTION", "JCT" },
                { "JUNCTIONS", "JCTS" },
                { "JUNCTN", "JCT" },
                { "JUNCTON", "JCT" },
                { "KEY", "KY" },
                { "KEYS", "KYS" },
                { "KNOL", "KNL" },
                { "KNOLL", "KNL" },
                { "KNOLLS", "KNLS" },
                { "LA", "LN" },
                { "LAKE", "LK" },
                { "LAKES", "LKS" },
                { "LANDING", "LNDG" },
                { "LANE", "LN" },
                { "LANES", "LN" },
                { "LDGE", "LDG" },
                { "LIGHT", "LGT" },
                { "LIGHTS", "LGTS" },
                { "LNDNG", "LNDG" },
                { "LOAF", "LF" },
                { "LOCK", "LCK" },
                { "LOCKS", "LCKS" },
                { "LODG", "LDG" },
                { "LODGE", "LDG" },
                { "LOOPS", "LOOP" },
                { "MANOR", "MNR" },
                { "MANORS", "MNRS" },
                { "MEADOW", "MDW" },
                { "MEADOWS", "MDWS" },
                { "MEDOWS", "MDWS" },
                { "MILL", "ML" },
                { "MILLS", "MLS" },
                { "MISSION", "MSN" },
                { "MISSN", "MSN" },
                { "MNT", "MT" },
                { "MNTAIN", "MTN" },
                { "MNTN", "MTN" },
                { "MNTNS", "MTNS" },
                { "MOTORWAY", "MTWY" },
                { "MOUNT", "MT" },
                { "MOUNTAIN", "MTN" },
                { "MOUNTAINS", "MTNS" },
                { "MOUNTIN", "MTN" },
                { "MSSN", "MSN" },
                { "MTIN", "MTN" },
                { "NECK", "NCK" },
                { "ORCHARD", "ORCH" },
                { "ORCHRD", "ORCH" },
                { "OVERPASS", "OPAS" },
                { "OVL", "OVAL" },
                { "PARKS", "PARK" },
                { "PARKWAY", "PKWY" },
                { "PARKWAYS", "PKWY" },
                { "PARKWY", "PKWY" },
                { "PASSAGE", "PSGE" },
                { "PATHS", "PATH" },
                { "PIKES", "PIKE" },
                { "PINE", "PNE" },
                { "PINES", "PNES" },
                { "PK", "PARK" },
                { "PKWAY", "PKWY" },
                { "PKWYS", "PKWY" },
                { "PKY", "PKWY" },
                { "PLACE", "PL" },
                { "PLAIN", "PLN" },
                { "PLAINES", "PLNS" },
                { "PLAINS", "PLNS" },
                { "PLAZA", "PLZ" },
                { "PLZA", "PLZ" },
                { "POINT", "PT" },
                { "POINTS", "PTS" },
                { "PORT", "PRT" },
                { "PORTS", "PRTS" },
                { "PRAIRIE", "PR" },
                { "PRARIE", "PR" },
                { "PRK", "PARK" },
                { "PRR", "PR" },
                { "RAD", "RADL" },
                { "RADIAL", "RADL" },
                { "RADIEL", "RADL" },
                { "RANCH", "RNCH" },
                { "RANCHES", "RNCH" },
                { "RAPID", "RPD" },
                { "RAPIDS", "RPDS" },
                { "RDGE", "RDG" },
                { "REST", "RST" },
                { "RIDGE", "RDG" },
                { "RIDGES", "RDGS" },
                { "RIVER", "RIV" },
                { "RIVR", "RIV" },
                { "RNCHS", "RNCH" },
                { "ROAD", "RD" },
                { "ROADS", "RDS" },
                { "ROUTE", "RTE" },
                { "RVR", "RIV" },
                { "SHOAL", "SHL" },
                { "SHOALS", "SHLS" },
                { "SHOAR", "SHR" },
                { "SHOARS", "SHRS" },
                { "SHORE", "SHR" },
                { "SHORES", "SHRS" },
                { "SKYWAY", "SKWY" },
                { "SPNG", "SPG" },
                { "SPNGS", "SPGS" },
                { "SPRING", "SPG" },
                { "SPRINGS", "SPGS" },
                { "SPRNG", "SPG" },
                { "SPRNGS", "SPGS" },
                { "SPURS", "SPUR" },
                { "SQR", "SQ" },
                { "SQRE", "SQ" },
                { "SQRS", "SQS" },
                { "SQU", "SQ" },
                { "SQUARE", "SQ" },
                { "SQUARES", "SQS" },
                { "STATION", "STA" },
                { "STATN", "STA" },
                { "STN", "STA" },
                { "STR", "ST" },
                { "STRAV", "STRA" },
                { "STRAVE", "STRA" },
                { "STRAVEN", "STRA" },
                { "STRAVENUE", "STRA" },
                { "STRAVN", "STRA" },
                { "STREAM", "STRM" },
                { "STREET", "ST" },
                { "STREETS", "STS" },
                { "STREME", "STRM" },
                { "STRT", "ST" },
                { "STRVN", "STRA" },
                { "STRVNUE", "STRA" },
                { "SUMIT", "SMT" },
                { "SUMITT", "SMT" },
                { "SUMMIT", "SMT" },
                { "TERR", "TER" },
                { "TERRACE", "TER" },
                { "THROUGHWAY", "TRWY" },
                { "TPK", "TPKE" },
                { "TR", "TRL" },
                { "TRACE", "TRCE" },
                { "TRACES", "TRCE" },
                { "TRACK", "TRAK" },
                { "TRACKS", "TRAK" },
                { "TRAFFICWAY", "TRFY" },
                { "TRAIL", "TRL" },
                { "TRAILS", "TRL" },
                { "TRK", "TRAK" },
                { "TRKS", "TRAK" },
                { "TRLS", "TRL" },
                { "TRNPK", "TPKE" },
                { "TRPK", "TPKE" },
                { "TUNEL", "TUNL" },
                { "TUNLS", "TUNL" },
                { "TUNNEL", "TUNL" },
                { "TUNNELS", "TUNL" },
                { "TUNNL", "TUNL" },
                { "TURNPIKE", "TPKE" },
                { "TURNPK", "TPKE" },
                { "UNDERPASS", "UPAS" },
                { "UNION", "UN" },
                { "UNIONS", "UNS" },
                { "VALLEY", "VLY" },
                { "VALLEYS", "VLYS" },
                { "VALLY", "VLY" },
                { "VDCT", "VIA" },
                { "VIADCT", "VIA" },
                { "VIADUCT", "VIA" },
                { "VIEW", "VW" },
                { "VIEWS", "VWS" },
                { "VILL", "VLG" },
                { "VILLAG", "VLG" },
                { "VILLAGE", "VLG" },
                { "VILLAGES", "VLGS" },
                { "VILLE", "VL" },
                { "VILLG", "VLG" },
                { "VILLIAGE", "VLG" },
                { "VIST", "VIS" },
                { "VISTA", "VIS" },
                { "VLLY", "VLY" },
                { "VST", "VIS" },
                { "VSTA", "VIS" },
                { "WALKS", "WALK" },
                { "WELL", "WL" },
                { "WELLS", "WLS" },
                { "WY", "WAY" }
            };

        private static readonly Dictionary<string, string> UnrangedSubunits =
            new Dictionary<string, string>
            {
                { "BASEMENT", "BSMT" },
                { "FRONT", "FRNT" },
                { "LOBBY", "LBBY" },
                { "LOWER", "LOWR" },
                { "OFFICE", "OFC" },
                { "PENTHOUSE", "PH" },
                { "REAR", "REAR" },
                { "SIDE", "SIDE" },
                { "UPPER", "UPPR" }
            };

        private static readonly Dictionary<string, string> RangedSubunits =
            new Dictionary<string, string>
            {
                { "SUITE", "STE" },
                { "APARTMENT", "APT" },
                { "DEPARTMENT", "DEPT" },
                { "ROOM", "RM" },
                { "FLOOR?", "FL" },
                { "UNIT", "UNIT" },
                { "BUILDING", "BLDG" },
                { "HANGAR", "HNGR" },
                { "KEY", "KEY" },
                { "LOT", "LOT" },
                { "PIER", "PIER" },
                { "SLIP", "SLIP" },
                { "SPACE?", "SPACE" },
                { "STOP", "STOP" },
                { "TRAILER", "TRLR" },
                { "BOX", "BOX" }
            };
    }
}
