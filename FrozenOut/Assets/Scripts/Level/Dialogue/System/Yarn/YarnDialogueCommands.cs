﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using Yarn.Unity;

namespace Scripts.Level.Dialogue.Runner.YarnSpinner
{
    public class YarnDialogueCommands : MonoBehaviour
    {
        public YarnDialogueSystem YarnSystem;

        private DialogueRunner DialogueRunner => YarnSystem.DialogueRunner;

        [YarnCommand("giveitem")]
        public void PickItem(string itemVariableName, int quantity)
        {
            YarnSystem.PickItem(itemVariableName, quantity);
        }

        [YarnCommand("useitem")]
        public void UseItem(string itemVariableName, int quantity)
        {
            YarnSystem.UseItem(itemVariableName, quantity);
        }

        [YarnCommand("setanim")]
        public void SetAnimation(string npcName, string animation)
        {
            YarnSystem.SetNPCAnimation(npcName, animation);
        }

        [YarnCommand("setanimall")]
        public void SetAnimationAll(string npcName, string animation)
        {
            YarnSystem.SetNPCAnimationWithSimilarName(npcName, animation);
        }

        [YarnCommand("stopanim")]
        public void StopAnimation(string npcName)
        {
            YarnSystem.StopNPCAnimation(npcName);
        }

        [YarnCommand("stopanimall")]
        public void StopAnimationAll(string npcName)
        {
            YarnSystem.StopNPCAnimationWithSimilarName(npcName);
        }
    }
}