package patches.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.CommitStatusPublisher
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.commitStatusPublisher
import jetbrains.buildServer.configs.kotlin.v2019_2.ui.*

/*
This patch script was generated by TeamCity on settings change in UI.
To apply the patch, change the buildType with id = 'WindowsNetFxTesting_2012r2'
accordingly, and delete the patch script.
*/
changeBuildType(RelativeId("WindowsNetFxTesting_2012r2")) {
    vcs {
        add(DslContext.settingsRoot.id!!)
    }

    features {
        val feature1 = find<CommitStatusPublisher> {
            commitStatusPublisher {
                publisher = github {
                    githubUrl = "https://api.github.com"
                    authType = personalToken {
                        token = "%commitStatusPublisher.apiKey%"
                    }
                }
            }
        }
        feature1.apply {
            publisher = github {
                githubUrl = "https://api.github.com"
                authType = personalToken {
                    token = "credentialsJSON:d2d6ff31-56f1-4893-a448-f7a517da6c88"
                }
            }
        }
    }
}
