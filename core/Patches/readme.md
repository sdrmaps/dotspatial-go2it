Directory for storage of all localized patches that have been applied to DotSpatial. Primarily this is for archive purposes as the majority of these will have been submitted to the DotSpatial project and are awaiting integration into the mainline branch. Descriptions are as follows:

---

__ProjectionInfo-fixes-equals-comparison.patch__

Discrepancies between ESRI WKT and Proj4 string representations as well as inconsistencies between American and BIPM spelling cause projections that are actually the same to return as false. Patch remedies this by comparing only actual values and disregards naming conventions entirely.