Directory for storage of all localized patches that have been applied to DotSpatial. Primarily this is for archive purposes as the majority of these will have been submitted to the DotSpatial project and are awaiting integration into the mainline branch. Descriptions are as follows:

---

__ProjectionInfo-fixes-Equals()-comparison__

Discrepancies between ESRI WKT and Proj4 string representations as well as inconsistencies between American and BIPM spelling cause projections that are actually the same to return as false. Patch remedies this by comparing only actual values and disregards naming conventions entirely.

__MapFrame-fixes-OnExtentChanged()-next-previous-stack-additions__

This patch fixes which extent changes are allow to be added to the next/previous extent stacks OnExtentChanged() event. 'View' changes are the result of operations such as panning or window resizes. 'View-extent' changes are granularity changes such as zooming in or out. Only 'view-extent' changes should be added to the next/previous extent stacks. However, due to the design of the ZoomIn() tool when a point is click as opposed to a box drawn a 'view' change results. Because we want to add this extent change to the next/previous stacks this patch also provides a method of adding 'view' changes when needed.