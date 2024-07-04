using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared._EstacaoPirata.Cards.Card;

/// <summary>
/// This handles...
/// </summary>
public sealed class CardSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<CardComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<CardComponent, GetVerbsEvent<AlternativeVerb>>(AddTurnOnVerb);
        SubscribeLocalEvent<CardComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<CardComponent, UseInHandEvent>(OnUse);

    }

    private void OnExamined(EntityUid uid, CardComponent component, ExaminedEvent args)
    {
        if (args.IsInDetailsRange && !component.Flipped)
        {
            args.PushMarkup(Loc.GetString("card-examined", ("target",  Loc.GetString(component.Name))));
        }
    }

    private void OnInteractUsing(EntityUid uid, CardComponent cardComponent, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp(args.Used, out CardComponent? _))
        {
            return;
        }

        args.Handled = true;
    }

    private void AddTurnOnVerb(EntityUid uid, CardComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract || args.Hands == null)
            return;

        args.Verbs.Add(new AlternativeVerb()
        {
            Act = () => FlipCard(uid, component),
            Text = Loc.GetString("cards-verb-flip"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/flip.svg.192dpi.png")),
            Priority = 1
        });
    }

    private void OnUse(EntityUid uid, CardComponent comp, UseInHandEvent args)
    {
        if (args.Handled)
            return;

        FlipCard(uid, comp);
        args.Handled = true;
    }

    private void FlipCard(EntityUid uid, CardComponent component)
    {
        component.Flipped = !component.Flipped;
        Dirty(uid, component);
        RaiseLocalEvent(uid, new CardComponent.CardFlipUpdatedEvent());
    }
}
