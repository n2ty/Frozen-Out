﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

using Yarn.Unity;

using Scripts.Level.Dialogue.Text;
using Scripts.Level.Dialogue.Text.Tag;

namespace Scripts.Level.Dialogue.YarnSpinner
{
    [RequireComponent(typeof(Canvas))]
    public class YarnDialogueController : DialogueUIBehaviour
    {
        public YarnManager DialogueManager;

        public Canvas DialogueCanvas;
        public DialogueStyle DefaultStyle;

        private bool IsOpen => DialogueCanvas.enabled;

        private const string PlayerName = "Pol";
        private const string DefaultLineSeparator = ":";
        private const float LetterDelay = 0.1f;
        private const float NextDialogueDelay = 0.3f;

        private bool UserRequestedAllLine;
        private bool UserRequestedNextLine;

        private IDictionary<string, DialogueStyle> Styles;

        void Awake()
        {
            Close();
        }

        private void Start()
        {
            SetStyles();
        }

        void Update()
        {
            if (DialogueManager.IsRunning() && Input.GetKeyDown(DialogueManager.GetNextDialogueKey()))
            {
                UserRequestedAllLine = true;
                UserRequestedNextLine = true;
            }
        }

        public void Open()
        {
            DialogueCanvas.enabled = true;
        }

        public void Close()
        {
            DialogueCanvas.enabled = false;
        }

        public override void DialogueStart()
        {
            Open();

            OnDialogueStart();
        }

        public override void DialogueComplete()
        {
            OnDialogueEnd();

            Close();
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

            SeparateNameAndDialogue(text, out string characterName, out string characterDialogue);

            OnNameLineUpdate(characterName);

            DialogueStyle characterStyle = GetUpdatedStyle(characterName);

            OnStyleLineUpdate(characterStyle);

            float currentLetterDelay = characterStyle.Delay;

            if (currentLetterDelay > 0.0f)
            {
                // Antes de hacer nada se analiza el texto y se clasifican internamente las partes con tags y las simples
                IDialogueText completeCharacterDialogue = ComplexDialogueText.AnalyzeText(characterDialogue);

                UserRequestedAllLine = false;

                foreach (string currentText in completeCharacterDialogue.Parse())
                {
                    OnDialogueLineUpdate(currentText);

                    if (UserRequestedAllLine)
                    {
                        OnDialogueLineUpdate(characterDialogue);
                        break;
                    }

                    yield return new WaitForSeconds(currentLetterDelay);
                }
            }
            else
            {
                OnDialogueLineUpdate(characterDialogue);
            }

            OnLineFinishDisplaying();

            yield return new WaitForSeconds(NextDialogueDelay);

            UserRequestedNextLine = false;

            while(!UserRequestedNextLine)
            {
                yield return null;
            }

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

        public void AddStyle(string characterName, DialogueStyle characterStyle)
        {
            if (characterStyle != null)
            {
                Styles[characterName] = characterStyle;
            }
        }

        private void SeparateNameAndDialogue(string text, out string name, out string dialogue)
        {
            int indexOfNameSeparator = text.IndexOf(DefaultLineSeparator);
            name = text.Substring(0, indexOfNameSeparator);
            dialogue = text.Substring(indexOfNameSeparator + 2);
        }

        private void SetStyles()
        {
            Styles = new Dictionary<string, DialogueStyle>();
            AddStyle(PlayerName, DefaultStyle);
        }

        private DialogueStyle GetStyle(string characterName)
        {
            DialogueStyle characterStyle = DefaultStyle;
            if(Styles.ContainsKey(characterName))
            {
                characterStyle = Styles[characterName];
            }

            return characterStyle;
        }

        private DialogueStyle GetUpdatedStyle(string characterName)
        {
            DialogueStyle style = GetStyle(characterName);

            style.UpdateDelay(LetterDelay);
            style.UpdateSize(DialogueManager.GetTextSize());
            style.UpdateFont(DefaultStyle.Font);

            return style;
        }

        #region Events
        public DialogueRunner.StringUnityEvent LineNameUpdated;
        public DialogueRunner.StringUnityEvent LineDialogueUpdated;
        public StyleUnityEvent LineStyleUpdated;

        private void OnNameLineUpdate(string nameToDisplay)
        {
            LineNameUpdated?.Invoke(nameToDisplay);
        }

        private void OnDialogueLineUpdate(string dialogueToDisplay)
        {
            LineDialogueUpdated?.Invoke(dialogueToDisplay);
        }

        private void OnStyleLineUpdate(DialogueStyle dialogueStyle)
        {
            LineStyleUpdated?.Invoke(dialogueStyle);
        }
        #endregion
    }

    [Serializable]
    public class StyleUnityEvent : UnityEvent<DialogueStyle> { }
}