package patches.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.ui.*

/*
This patch script was generated by TeamCity on settings change in UI.
To apply the patch, remove the buildType with id = 'NetcoreTesting_Sles'
from your code, and delete the patch script.
*/
deleteBuildType(RelativeId("NetcoreTesting_Sles"))

