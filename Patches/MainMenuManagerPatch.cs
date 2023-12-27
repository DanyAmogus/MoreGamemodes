using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MoreGamemodes
{
    [HarmonyPatch(typeof(MainMenuManager))]
    public class MainMenuManagerPatch
    {
        public static Transform rightpanel;
        private static PassiveButton template;
        private static PassiveButton discordButton;
        private static PassiveButton gitHubButton;
        public static SpriteRenderer MG_Logo;

        [HarmonyPatch(nameof(MainMenuManager.Start)), HarmonyPostfix, HarmonyPriority(Priority.VeryHigh)]
        public static void StartPriorityPostfix(MainMenuManager __instance)
        {
            var rightpanel = __instance.gameModeButtons.transform.parent;
            var logoObject = new GameObject("titleLogo_TOH");
            var logoTransform = logoObject.transform;
            MG_Logo = logoObject.AddComponent<SpriteRenderer>();
            logoTransform.parent = rightpanel;
            logoTransform.localPosition = new(0f, 0.15f, 1f);
            logoTransform.localScale *= 1.2f;
            MG_Logo.sprite = Utils.LoadSprite("MoreGamemodes.Resources.MoreGamemodes-Logo.png", 400f);
        }

        [HarmonyPatch(nameof(MainMenuManager.Start)), HarmonyPostfix, HarmonyPriority(Priority.Normal)]
        public static void StartPostfix(MainMenuManager __instance)
        {
            if (rightpanel == null) rightpanel = __instance.gameModeButtons.transform.parent;
            if (template == null) template = __instance.quitButton;
            if (rightpanel == null || template == null) return;
            if (discordButton == null)
            {
                discordButton = CreateButton(
                    "DiscordButton",
                    new(-1f, -1f, 1f),
                    new(88, 101, 242, byte.MaxValue),
                    new(148, 161, byte.MaxValue, byte.MaxValue),
                    () => Application.OpenURL("https://discord.gg/jJe5kPpbFJ"),
                    "Discord");
            }

            if (gitHubButton == null)
            {
                gitHubButton = CreateButton(
                    "GitHubButton",
                    new(1f, -1f, 1f),
                    new(153, 153, 153, byte.MaxValue),
                    new(209, 209, 209, byte.MaxValue),
                    () => Application.OpenURL("https://github.com/Rabek009/MoreGamemodes"),
                    "GitHub");
            }

            var howToPlayButton = __instance.howToPlayButton;
            var freeplayButton = howToPlayButton.transform.parent.Find("FreePlayButton");
            if (freeplayButton != null)
            {
                freeplayButton.gameObject.SetActive(false);
            }
            howToPlayButton.transform.SetLocalX(0);
        }

        private static PassiveButton CreateButton(string name, Vector3 localPosition, Color32 normalColor, Color32 hoverColor, Action action, string label, Vector2? scale = null)
        {
            var obj = new GameObject("");
            obj.transform.parent = rightpanel;
            obj.transform.localPosition = new(0f, 0.15f, 1f);
            obj.transform.localScale *= 1.2f;
            var button = Object.Instantiate(template, obj.transform);
            button.name = name;
            Object.Destroy(button.GetComponent<AspectPosition>());
            button.transform.localPosition = localPosition;

            button.OnClick = new();
            button.OnClick.AddListener(action);

            var buttonText = button.transform.Find("FontPlacer/Text_TMP").GetComponent<TMP_Text>();
            var translator = buttonText.GetComponent<TextTranslatorTMP>();
            if (translator != null)
            {
                Object.Destroy(translator);
            }
            buttonText.fontSize = buttonText.fontSizeMax = buttonText.fontSizeMin = 3.5f;
            buttonText.enableWordWrapping = false;
            buttonText.text = label;
            var normalSprite = button.inactiveSprites.GetComponent<SpriteRenderer>();
            var hoverSprite = button.activeSprites.GetComponent<SpriteRenderer>();
            normalSprite.color = normalColor;
            hoverSprite.color = hoverColor;

            var container = buttonText.transform.parent;
            Object.Destroy(container.GetComponent<AspectPosition>());
            Object.Destroy(buttonText.GetComponent<AspectPosition>());
            container.SetLocalX(0f);
            buttonText.transform.SetLocalX(0f);
            buttonText.horizontalAlignment = HorizontalAlignmentOptions.Center;

            var buttonCollider = button.GetComponent<BoxCollider2D>();
            if (scale.HasValue)
            {
                normalSprite.size = hoverSprite.size = buttonCollider.size = scale.Value;
            }
            buttonCollider.offset = new(0f, 0f);

            return button;
        }

        [HarmonyPatch(nameof(MainMenuManager.OpenGameModeMenu))]
        [HarmonyPatch(nameof(MainMenuManager.OpenAccountMenu))]
        [HarmonyPatch(nameof(MainMenuManager.OpenCredits))]
        [HarmonyPostfix]
        public static void OpenMenuPostfix()
        {
            if (MG_Logo != null)
            {
                MG_Logo.gameObject.SetActive(false);
            }
        }
        [HarmonyPatch(nameof(MainMenuManager.ResetScreen)), HarmonyPostfix]
        public static void ResetScreenPostfix()
        {
            if (MG_Logo != null)
            {
                MG_Logo.gameObject.SetActive(true);
            }
        }
    }
}