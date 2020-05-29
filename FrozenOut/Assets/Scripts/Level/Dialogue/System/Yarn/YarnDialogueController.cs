﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

using Yarn.Unity;

namespace Scripts.Level.Dialogue.Runner.YarnSpinner
{
    public class YarnDialogueController : DialogueUIBehaviour
    {
        public YarnDialogueSystem DialogueSystem;

        private const string StyleSeparator = "++";
        private const string DialogueSeparator = ": ";

        private bool RequestedNextLine;

        public void RequestNextLine()
        {
            RequestedNextLine = true;
        }

        public override void DialogueStart()
        {
            OnDialogueStart();
        }

        public override void DialogueComplete()
        {
            OnDialogueEnd();
        }

        public override Yarn.Dialogue.HandlerExecutionType RunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, System.Action onLineComplete)
        {
            StartCoroutine(DoRunLine(line, localisationProvider, onLineComplete));
            return Yarn.Dialogue.HandlerExecutionType.PauseExecution;
        }

        private IEnumerator DoRunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, System.Action onComplete)
        {
            OnLineStart();

            string text = localisationProvider.GetLocalisedTextForLine(line);

            // Sanity check
            if (text == null) {
                Debug.LogWarning($"Line {line.ID} doesn't have any localised text.");
                text = line.ID;
            }

            SeparateNameAndDialogue(text, out string characterStyleName, out string characterName, out string characterDialogue);

            OnLineStyleUpdated(characterStyleName);
            OnNameLineUpdate(characterName);
            OnDialogueLineUpdate(characterDialogue);

            while(!RequestedNextLine)
            {
                yield return null;
            }
            RequestedNextLine = false;

            yield return new WaitForEndOfFrame();

            OnLineEnd();

            onComplete();
        }

        public override void RunOptions(Yarn.OptionSet optionSet, ILineLocalisationProvider localisationProvider, System.Action<int> onOptionSelected)
        {
            
        }

        public override Yarn.Dialogue.HandlerExecutionType RunCommand(Yarn.Command command, System.Action onCommandComplete)
        {
            return Yarn.Dialogue.HandlerExecutionType.ContinueExecution;
        }

        private void SeparateNameAndDialogue(string text, out string style, out string name, out string dialogue)
        {
            int indexOfStyleSeparator = text.IndexOf(StyleSeparator);
            int indexOfDialogueSeparator = text.IndexOf(DialogueSeparator);

            if (indexOfStyleSeparator == -1) // No hay estilo adicional
            {
                name = text.Substring(0, indexOfDialogueSeparator);
                style = name;
            }
            else
            {
                name = text.Substring(0, indexOfStyleSeparator);

                int styleLength = indexOfDialogueSeparator - (indexOfStyleSeparator + StyleSeparator.Length);
                style = text.Substring(indexOfStyleSeparator + StyleSeparator.Length, styleLength);
            }
            dialogue = text.Substring(indexOfDialogueSeparator + DialogueSeparator.Length);
        }

        #region Events
        public DialogueRunner.StringUnityEvent LineStyleUpdated;
        public DialogueRunner.StringUnityEvent LineNameUpdated;
        public DialogueRunner.StringUnityEvent LineDialogueUpdated;

        private void OnLineStyleUpdated(string styleName)
        {
            LineStyleUpdated?.Invoke(styleName);
        }

        private void OnNameLineUpdate(string name)
        {
            LineNameUpdated?.Invoke(name);
        }

        private void OnDialogueLineUpdate(string dialogue)
        {
            LineDialogueUpdated?.Invoke(dialogue);
        }
        #endregion
    }
}