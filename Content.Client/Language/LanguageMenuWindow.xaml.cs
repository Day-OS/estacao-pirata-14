using Content.Shared.Language;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Language;

[GenerateTypedNameReferences]
public sealed partial class LanguageMenuWindow : DefaultWindow
{
    private readonly List<(string language, Button button)> _buttons = new();

    public LanguageMenuWindow(LanguageMenuUserInterface ui)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
    }

    public void UpdateState(SharedLanguageSystem.LanguageMenuState state)
    {
        // TODO: this is a placeholder
        CurrentLanguageLabel.Text = "Current language: " + state.CurrentLanguage;

        OptionsContainer.DisposeAllChildren();
        _buttons.Clear();

        foreach (var language in state.Options)
        {
            var entry = MakeLanguageEntry(language);
            OptionsContainer.AddChild(entry);
        }

        foreach (var entry in _buttons)
        {
            // Disable the button for the current language, if any.
            entry.button.Disabled = state.CurrentLanguage == entry.language;
        }
    }

    private BoxContainer MakeLanguageEntry(string language)
    {
        var container = new BoxContainer();
        container.Orientation = BoxContainer.LayoutOrientation.Horizontal;
        container.HorizontalExpand = true;
        container.SeparationOverride = 4;

        var name = new Label();
        name.Text = language;

        var description = new Label();
        description.Text = "TODO: descriptions";

        var button = new Button();
        button.Text = "Choose";
        button.OnPressed += _ => OnLanguageChosen(language);

        _buttons.Add((language, button));

        container.AddChild(name);
        container.AddChild(description);
        container.AddChild(button);

        return container;
    }

    private void OnLanguageChosen(string id)
    {
        // TODO: send some message to the server or execute a command, idk
    }
}
