using Content.Shared.Preferences;
using Robust.Shared.Prototypes;

namespace Content.Shared.Roles
{
    [Prototype("startingGear")]
    public sealed partial class StartingGearPrototype : IPrototype
    {
        [DataField]
        public Dictionary<string, EntProtoId> Equipment = new();

        /// <summary>
        /// if empty, there is no skirt override - instead the uniform provided in equipment is added.
        /// </summary>
        [DataField]
        public EntProtoId? InnerClothingSkirt;

        [DataField]
        public EntProtoId? Satchel;

        [DataField]
        public EntProtoId? Duffelbag;

        [DataField]
        public List<EntProtoId> Inhand = new(0);

        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = string.Empty;

        public string GetGear(string slot, HumanoidCharacterProfile? profile)
        {
            if (profile != null)
            {
                if (slot == "jumpsuit" && profile.Clothing == ClothingPreference.Jumpskirt && !string.IsNullOrEmpty(_innerClothingSkirt))
                    return _innerClothingSkirt;
                if (slot == "back" && profile.Backpack == BackpackPreference.Satchel && !string.IsNullOrEmpty(_satchel))
                    return _satchel;
                if (slot == "back" && profile.Backpack == BackpackPreference.Duffelbag && !string.IsNullOrEmpty(_duffelbag))
                    return _duffelbag;


                // Handles equipping all crew with underwear without putting it in every file.
                // Checks for skirt settings, if skirt = true, equip with panties and bra
                // if skirt = false, equip with boxers and shirt.
                // Using else caused weird issues I didn't feel like dealing with -Psp

                if (slot == "underpants" && profile.Clothing != ClothingPreference.Jumpskirt && string.IsNullOrEmpty(_underpants))
                    return "ClothingUnderboxer_briefs";
                if (slot == "underpants" && profile.Clothing == ClothingPreference.Jumpskirt && string.IsNullOrEmpty(_underpantsskirt))
                    return "ClothingUnderpanties";

                if (slot == "undershirt" && profile.Clothing != ClothingPreference.Jumpskirt && string.IsNullOrEmpty(_undershirt))
                    return "ClothingUnderundershirt";
                if (slot == "undershirt" && profile.Clothing == ClothingPreference.Jumpskirt && string.IsNullOrEmpty(_undershirtskirt))
                    return "ClothingUnderbra";

                if (slot == "socks" && string.IsNullOrEmpty(_undersocks))
                    return "ClothingUnderSocks_norm";

                // Handles custom underwear per role.

                if (slot == "underpants" && profile.Clothing != ClothingPreference.Jumpskirt && !string.IsNullOrEmpty(_underpants) && _underpants != "empty")
                    return _underpants;
                if (slot == "underpants" && profile.Clothing == ClothingPreference.Jumpskirt && !string.IsNullOrEmpty(_underpantsskirt) && _underpantsskirt != "empty")
                    return _underpantsskirt;

                if (slot == "undershirt" && profile.Clothing != ClothingPreference.Jumpskirt && !string.IsNullOrEmpty(_undershirt) && _undershirt != "empty")
                    return _undershirt;
                if (slot == "undershirt" && profile.Clothing == ClothingPreference.Jumpskirt && !string.IsNullOrEmpty(_undershirtskirt) && _undershirtskirt != "empty")
                    return _undershirtskirt;

                if (slot == "socks" && !string.IsNullOrEmpty(_undersocks) && _undersocks != "empty")
                    return _undersocks;
            }

            return Equipment.TryGetValue(slot, out var equipment) ? equipment : string.Empty;
        }
    }
}
