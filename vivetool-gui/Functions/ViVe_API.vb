﻿'ViVeTool GUI - Windows Feature Control GUI for ViVeTool
'Copyright (C) 2023 Peter Strick
'
'This program is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.
'
'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.
'
'You should have received a copy of the GNU General Public License
'along with this program.  If not, see <https://www.gnu.org/licenses/>.
Option Strict On
Imports Albacore.ViVe, Albacore.ViVe.Exceptions, Albacore.ViVe.NativeStructs, Albacore.ViVe.NativeEnums

''' <summary>
''' Class that contains all Functions to work with the ViVe API
''' </summary>
Partial Class ViVe_API

    ''' <summary>
    ''' Class that contains all Functions to Set and Query ViVeTool Features
    ''' </summary>
    Public Class Feature
        ''' <summary>
        ''' Internal Function that set's Windows Feature Configuration accordingly with the specified ID, Variant, State and Priority
        ''' </summary>
        ''' <param name="ID">ViVeTool ID</param>
        ''' <param name="ID_Variant">ViVeTool ID Variant</param>
        ''' <param name="State">Feature State, can be either Enabled, Disabled or Default</param>
        ''' <param name="Priority">Feature Priority</param>
        ''' <param name="Operation">Feature Operation. Can be either VariantState and FeatureState for Enable/Disable, or alternatively ResetState for Reset</param>
        ''' <param name="Type">The Feature Configuration Store to choose. Can be either Runtime or Boot. Using both is recommended</param>
        ''' <returns>An Integer corresponding to the Seccess Code of FeatureManager.SetFeatureConfigurations()</returns>
        Shared Function SetConfig(ID As UInteger, ID_Variant As UInteger, State As RTL_FEATURE_ENABLED_STATE,
                                  Priority As RTL_FEATURE_CONFIGURATION_PRIORITY, Operation As RTL_FEATURE_CONFIGURATION_OPERATION,
                                  Type As RTL_FEATURE_CONFIGURATION_TYPE) As Integer
            Dim _configs(0) As RTL_FEATURE_CONFIGURATION_UPDATE

            _configs(0) = New RTL_FEATURE_CONFIGURATION_UPDATE() With {
               .FeatureId = ID,
               .EnabledState = State,
               .EnabledStateOptions = RTL_FEATURE_ENABLED_STATE_OPTIONS.WexpConfig,
               .[Variant] = ID_Variant,
               .VariantPayload = ID_Variant,
               .VariantPayloadKind = RTL_FEATURE_VARIANT_PAYLOAD_KIND.External,
               .Priority = Priority,
               .Operation = Operation
            }

            Return FeatureManager.SetFeatureConfigurations(_configs, Type)
        End Function

        ''' <summary>
        ''' Enables ViVeTool Features using a specified ID and optionally a Variant
        ''' </summary>
        ''' <param name="ID">ViVeTool ID</param>
        ''' <param name="ID_Variant">ViVeTool ID Variant</param>
        Public Shared Sub Enable(ID As UInteger, Optional ID_Variant As UInteger = 0)
            Try
                ' Set the Feature to it's corresponding Enabled State in the Runtime Store
                Dim result_runtime = SetConfig(ID, ID_Variant, RTL_FEATURE_ENABLED_STATE.Enabled, RTL_FEATURE_CONFIGURATION_PRIORITY.User,
                                       RTL_FEATURE_CONFIGURATION_OPERATION.FeatureState Or RTL_FEATURE_CONFIGURATION_OPERATION.VariantState,
                                       RTL_FEATURE_CONFIGURATION_TYPE.Runtime)

                ' Set the Feature to it's corresponding Enabled State in the Boot Store (Feature becomes persistent)
                Dim result_boot = SetConfig(ID, ID_Variant, RTL_FEATURE_ENABLED_STATE.Enabled, RTL_FEATURE_CONFIGURATION_PRIORITY.User,
                                       RTL_FEATURE_CONFIGURATION_OPERATION.FeatureState Or RTL_FEATURE_CONFIGURATION_OPERATION.VariantState,
                                       RTL_FEATURE_CONFIGURATION_TYPE.Boot)

                ' Check if the Return Code for both is 0, meaning Success
                If result_runtime = 0 AndAlso result_boot = 0 Then
                    RadTD.ShowDialog(My.Resources.SetConfig_Success,
                    String.Format(My.Resources.SetConfig_SuccessfullySetFeatureID, ID, RTL_FEATURE_ENABLED_STATE.Enabled.ToString),
                    Nothing, RadTaskDialogIcon.ShieldSuccessGreenBar)
                Else ' If not then show an Error
                    RadTD.ShowDialog(My.Resources.Error_Error,
                    String.Format(My.Resources.Error_SetConfig, ID, RTL_FEATURE_ENABLED_STATE.Enabled.ToString),
                    Nothing, RadTaskDialogIcon.Error, ExpandedText:=$"The Functions returned {result_runtime & result_boot}. Expected 00")
                End If
            Catch fpoe As FeaturePropertyOverflowException
                ' Used within ViVeTool so also catched here
                RadTD.ShowDialog($" {My.Resources.Error_AnErrorOccurred}", My.Resources.Error_AnErrorOccurred, fpoe.Message,
                                 RadTaskDialogIcon.Error, fpoe, fpoe.ToString, fpoe.ToString)
            Catch ex As Exception
                ' Catch any other Exception
                RadTD.ShowDialog($" {My.Resources.Error_AnExceptionOccurred}", My.Resources.Error_AnUnknownExceptionOccurred,
                Nothing, RadTaskDialogIcon.ShieldErrorRedBar, ex, ex.ToString, ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' Disables ViVeTool Features using a specified ID and optionally a Variant
        ''' </summary>
        ''' <param name="ID">ViVeTool ID</param>
        ''' <param name="ID_Variant">ViVeTool ID Variant</param>
        Public Shared Sub Disable(ID As UInteger, ID_Variant As UInteger)
            Try
                ' Set the Feature to it's corresponding Enabled State in the Runtime Store
                Dim result_runtime = SetConfig(ID, ID_Variant, RTL_FEATURE_ENABLED_STATE.Disabled, RTL_FEATURE_CONFIGURATION_PRIORITY.User,
                                       RTL_FEATURE_CONFIGURATION_OPERATION.FeatureState Or RTL_FEATURE_CONFIGURATION_OPERATION.VariantState,
                                       RTL_FEATURE_CONFIGURATION_TYPE.Runtime)

                ' Set the Feature to it's corresponding Enabled State in the Boot Store (Feature becomes persistent)
                Dim result_boot = SetConfig(ID, ID_Variant, RTL_FEATURE_ENABLED_STATE.Disabled, RTL_FEATURE_CONFIGURATION_PRIORITY.User,
                                       RTL_FEATURE_CONFIGURATION_OPERATION.FeatureState Or RTL_FEATURE_CONFIGURATION_OPERATION.VariantState,
                                       RTL_FEATURE_CONFIGURATION_TYPE.Boot)

                ' Check if the Return Code for both is 0, meaning Success
                If result_runtime = 0 AndAlso result_boot = 0 Then
                    RadTD.ShowDialog(My.Resources.SetConfig_Success,
                    String.Format(My.Resources.SetConfig_SuccessfullySetFeatureID, ID, RTL_FEATURE_ENABLED_STATE.Disabled.ToString),
                    Nothing, RadTaskDialogIcon.ShieldSuccessGreenBar)
                Else ' If not then show an Error
                    RadTD.ShowDialog(My.Resources.Error_Error,
                    String.Format(My.Resources.Error_SetConfig, ID, RTL_FEATURE_ENABLED_STATE.Disabled.ToString),
                    Nothing, RadTaskDialogIcon.Error, ExpandedText:=$"The Functions returned {result_runtime & result_boot}. Expected 00")
                End If
            Catch fpoe As FeaturePropertyOverflowException
                ' Used within ViVeTool so also catched here
                RadTD.ShowDialog($" {My.Resources.Error_AnErrorOccurred}", My.Resources.Error_AnErrorOccurred, fpoe.Message,
                                 RadTaskDialogIcon.Error, fpoe, fpoe.ToString, fpoe.ToString)
            Catch ex As Exception
                ' Catch any other Exception
                RadTD.ShowDialog($" {My.Resources.Error_AnExceptionOccurred}", My.Resources.Error_AnUnknownExceptionOccurred,
                Nothing, RadTaskDialogIcon.ShieldErrorRedBar, ex, ex.ToString, ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' Resets ViVeTool Features using a specified ID
        ''' </summary>
        ''' <param name="ID">ViVeTool ID</param>
        Public Shared Sub Reset(ID As UInteger)
            Try
                ' Set the Feature to it's corresponding Enabled State in the Runtime Store
                Dim result_runtime = SetConfig(ID, 0, RTL_FEATURE_ENABLED_STATE.Default, RTL_FEATURE_CONFIGURATION_PRIORITY.User,
                                       RTL_FEATURE_CONFIGURATION_OPERATION.ResetState, RTL_FEATURE_CONFIGURATION_TYPE.Runtime)

                ' Set the Feature to it's corresponding Enabled State in the Boot Store (Feature becomes persistent)
                Dim result_boot = SetConfig(ID, 0, RTL_FEATURE_ENABLED_STATE.Default, RTL_FEATURE_CONFIGURATION_PRIORITY.User,
                                       RTL_FEATURE_CONFIGURATION_OPERATION.ResetState, RTL_FEATURE_CONFIGURATION_TYPE.Boot)

                ' Check if the Return Code for both is 0, meaning Success
                If result_runtime = 0 AndAlso result_boot = 0 Then
                    RadTD.ShowDialog(My.Resources.SetConfig_Success,
                    String.Format(My.Resources.SetConfig_SuccessfullySetFeatureID, ID, RTL_FEATURE_ENABLED_STATE.Default.ToString),
                    Nothing, RadTaskDialogIcon.ShieldSuccessGreenBar)
                Else ' If not then show an Error
                    RadTD.ShowDialog(My.Resources.Error_Error,
                    String.Format(My.Resources.Error_SetConfig, ID, RTL_FEATURE_ENABLED_STATE.Default.ToString),
                    Nothing, RadTaskDialogIcon.Error, ExpandedText:=$"The Functions returned {result_runtime & result_boot}. Expected 00")
                End If
            Catch fpoe As FeaturePropertyOverflowException
                ' Used within ViVeTool so also catched here
                RadTD.ShowDialog($" {My.Resources.Error_AnErrorOccurred}", My.Resources.Error_AnErrorOccurred, fpoe.Message,
                                 RadTaskDialogIcon.Error, fpoe, fpoe.ToString, fpoe.ToString)
            Catch ex As Exception
                ' Catch any other Exception
                RadTD.ShowDialog($" {My.Resources.Error_AnExceptionOccurred}", My.Resources.Error_AnUnknownExceptionOccurred,
                Nothing, RadTaskDialogIcon.ShieldErrorRedBar, ex, ex.ToString, ex.ToString)
            End Try
        End Sub

        ''' <summary>
        ''' Querys the Enabled State of a ViVeTool Feature using a specified ID
        ''' </summary>
        ''' <param name="ID">ViVeTool ID</param>
        ''' <returns>ViVeTool ID Enabled State as String</returns>
        Public Shared Function Query(ID As UInteger) As String
            Dim _query = FeatureManager.QueryFeatureConfiguration(ID).GetValueOrDefault
            Return _query.EnabledState.ToString
        End Function

        ''' <summary>
        ''' Returns an Array of Enabled/Disabled Features on the current System
        ''' </summary>
        ''' <param name="Store">The Feature Configuration Store to use. Can be either Runtime or Boot</param>
        ''' <returns>A RTL_FEATURE_CONFIGURATION Array</returns>
        Public Shared Function QueryAll(Store As RTL_FEATURE_CONFIGURATION_TYPE) As RTL_FEATURE_CONFIGURATION()
            Return FeatureManager.QueryAllFeatureConfigurations(Store)
        End Function

        ''' <summary>
        ''' Windows keeps a Backup of Feature Configurations, known as the LKG Store and rolls back to it, if a Feature causes System instabillity.
        ''' 
        ''' The ViVe API has a function to automatically fix the LKG Store, if it has become corrupted due to a use-after-free bug in fcon.dll
        ''' </summary>
        ''' <returns>True if the Fix was successful, False if an Error occurred</returns>
        Public Shared Sub FixLastKnownGood()
            If FeatureManager.FixLKGStore() Then
                RadTD.ShowDialog(My.Resources.SetConfig_Success, My.Resources.ViVe_LKGStore_Repaired,
                                 Nothing, RadTaskDialogIcon.ShieldSuccessGreenBar)
            Else
                RadTD.ShowDialog(My.Resources.Generic_NotRequired, My.Resources.ViVe_LKGStore_NotRequired,
                                 Nothing, RadTaskDialogIcon.Information)
            End If
        End Sub

        ''' <summary>
        ''' This moves ViVeTool Features from the Service Priority to the User Priority. Features in the Service Priority are A/B Tests that can be added and removed remotely by Microsofts A/B Testing Program. Moving them to the User Priority makes them Permanent / not remotely modifiable.
        ''' </summary>
        Public Shared Sub FixPriority()
            Dim FixStatus_Runtime As Integer
            Dim FixStatus_Boot As Integer

            FixStatus_Runtime = FixPriority_HelperFunction(RTL_FEATURE_CONFIGURATION_TYPE.Runtime)
            FixStatus_Boot = FixPriority_HelperFunction(RTL_FEATURE_CONFIGURATION_TYPE.Boot)

            If FixStatus_Runtime = 0 AndAlso FixStatus_Boot = 0 Then
                RadTD.ShowDialog(My.Resources.SetConfig_Success, My.Resources.ViVe_Priorities_Fixed,
                                 Nothing, RadTaskDialogIcon.ShieldSuccessGreenBar)
            ElseIf FixStatus_Runtime = 1 AndAlso FixStatus_Boot = 1 Then
                RadTD.ShowDialog(My.Resources.Generic_NotRequired, My.Resources.ViVe_Priorities_NotRequired,
                                 Nothing, RadTaskDialogIcon.Information)
            ElseIf FixStatus_Runtime = 2 AndAlso FixStatus_Boot = 2 Then
                RadTD.ShowDialog(My.Resources.Error_Error, My.Resources.ViVe_Priorities_Error_Text, Nothing, RadTaskDialogIcon.Error,
                                 ExpandedText:=String.Format(My.Resources.ViVe_Priorities_Error_Expanded, FixStatus_Runtime & FixStatus_Boot))
            End If
        End Sub

        ''' <summary>
        ''' Helper Function for FixPriority(). Returns Status Codes depending on the Return of FixPriority_HelperFunction_SetConfig()
        ''' </summary>
        ''' <param name="configurationType">Feature Configuration Store to perform the Operation on. Can be either Runtime or Boot</param>
        ''' <returns>0 for Success, 1 if fixing is not required, 2 for any Errors</returns>
        Private Shared Function FixPriority_HelperFunction(configurationType As RTL_FEATURE_CONFIGURATION_TYPE) As Integer
            Dim Features = FeatureManager.QueryAllFeatureConfigurations(configurationType)
            Dim Fixes = FixPriority_Query(Features)

            If Fixes Is Nothing Then Return 1

            Dim FixStatus As Integer
            For Each FeatureFix In Fixes
                FixStatus = SetConfig(FeatureFix.FeatureId, FeatureFix.Variant, FeatureFix.EnabledState, FeatureFix.Priority,
                                      FeatureFix.Operation, configurationType)
            Next

            If FixStatus = 0 Then
                Return 0
            Else
                Return 2
            End If
        End Function

        ''' <summary>
        ''' Helper Function that determines if any Features require a Priority Fix
        ''' </summary>
        ''' <param name="configurations">A Feature Configuration Array returned from FeatureManager.QueryAllFeatureConfigurations()</param>
        ''' <returns>A Feature Configuration Update Array, or Nothing if no Features require fixing</returns>
        Private Shared Function FixPriority_Query(configurations As RTL_FEATURE_CONFIGURATION()) As RTL_FEATURE_CONFIGURATION_UPDATE()
            Dim configsToFix = configurations.Where(Function(x) x.Priority = RTL_FEATURE_CONFIGURATION_PRIORITY.Service AndAlso Not x.IsWexpConfiguration)
            If Not configsToFix.Any() Then Return Nothing

            Dim priorityFixUpdates(configsToFix.Count() * 2) As RTL_FEATURE_CONFIGURATION_UPDATE
            priorityFixUpdates(configsToFix.Count() * 2) = New RTL_FEATURE_CONFIGURATION_UPDATE()
            Dim updatesCreated = 0

            For Each cfg In configsToFix
                priorityFixUpdates(updatesCreated) = New RTL_FEATURE_CONFIGURATION_UPDATE() With {
                    .FeatureId = cfg.FeatureId,
                    .Priority = cfg.Priority,
                    .Operation = RTL_FEATURE_CONFIGURATION_OPERATION.ResetState
                }
                priorityFixUpdates(updatesCreated + 1) = New RTL_FEATURE_CONFIGURATION_UPDATE() With {
                    .FeatureId = cfg.FeatureId,
                    .Priority = RTL_FEATURE_CONFIGURATION_PRIORITY.User,
                    .EnabledState = cfg.EnabledState,
                    .[Variant] = cfg.[Variant],
                    .VariantPayloadKind = cfg.VariantPayloadKind,
                    .VariantPayload = cfg.VariantPayload,
                    .Operation = RTL_FEATURE_CONFIGURATION_OPERATION.FeatureState Or RTL_FEATURE_CONFIGURATION_OPERATION.VariantState
                }
                updatesCreated += 2
            Next

            Return priorityFixUpdates
        End Function
    End Class
End Class