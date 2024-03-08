using Content.Client.Language.Systems;
using Content.Shared.Language;
using Content.Shared.Language.Systems;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Console;
using Robust.Shared.Utility;
using static Content.Shared.Language.Systems.SharedLanguageSystem;

namespace Content.Client.Language;

[GenerateTypedNameReferences]
public sealed partial class LanguageMenuWindow : DefaultWindow
{
    [Dependency] private readonly IConsoleHost _consoleHost = default!;
    private readonly SharedLanguageSystem _language;

    private readonly List<EntryState> _entries = new();

    public LanguageMenuWindow()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
        _language = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SharedLanguageSystem>();

        Title = Loc.GetString("language-menu-window-title");
    }

    public void UpdateState(LanguageMenuStateMessage state)
    {
        var clanguage = _language.GetLanguage(state.CurrentLanguage);
        CurrentLanguageLabel.Text = Loc.GetString("language-menu-current-language", ("language", clanguage?.LocalizedName ?? "<error>"));

        OptionsList.RemoveAllChildren();
        _entries.Clear();

        foreach (var language in state.Options)
        {
            AddLanguageEntry(language);
        }

        // Disable the button for the currently chosen language
        foreach (var entry in _entries)
        {
            if (entry.button != null)
                entry.button.Disabled = entry.language == state.CurrentLanguage;
        }
    }

    private void AddLanguageEntry(string language)
    {
        var proto = _language.GetLanguage(language);
        var state = new EntryState { language = language };

        var container = new BoxContainer();
        container.Orientation = BoxContainer.LayoutOrientation.Vertical;

        // Create and add a header with the name and the button to select the language
        {
            var header = new BoxContainer();
            header.Orientation = BoxContainer.LayoutOrientation.Horizontal;

            header.Orientation = BoxContainer.LayoutOrientation.Horizontal;
            header.HorizontalExpand = true;
            header.SeparationOverride = 2;

            var name = new Label();
            name.Text = proto?.LocalizedName ?? "<error>";
            name.MinWidth = 50;
            name.HorizontalExpand = true;

            var button = new Button();
            button.Text = "Choose";
            button.OnPressed += _ => OnLanguageChosen(language);
            state.button = button;

            header.AddChild(name);
            header.AddChild(button);

            container.AddChild(header);
        }

        // Create and add a collapsible description
        {
            var body = new CollapsibleBody();
            body.HorizontalExpand = true;
            body.Margin = new Thickness(4f, 4f);

            var description = new RichTextLabel();
            description.SetMessage(proto?.LocalizedDescription ?? "<error>");
            description.HorizontalExpand = true;

            body.AddChild(description);

            var collapser = new Collapsible(Loc.GetString("language-menu-description-header"), body);
            collapser.Orientation = BoxContainer.LayoutOrientation.Vertical;
            collapser.HorizontalExpand = true;

            container.AddChild(collapser);
        }

        // Before adding, wrap the new container in a PanelContainer to give it a distinct look
        var wrapper = new PanelContainer();
        wrapper.StyleClasses.Add("PdaBorderRect");

        wrapper.AddChild(container);
        OptionsList.AddChild(wrapper);

        _entries.Add(state);
    }

    private void OnLanguageChosen(string id)
    {
        _consoleHost.ExecuteCommand("lsselectlang " + id);
    }

    private struct EntryState
    {
        public string language;
        public Button? button;
    }
}
