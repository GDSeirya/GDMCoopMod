using System.Collections.Generic;
using Game;

public static class LanguageManager
{
    private static string CurrentLang
    {
        get
        {
            string suffix = GameManager.GetTextLanguageSuffix();

            switch (suffix)
            {
                case "ja": return "ja";
                case "ko": return "ko";
                case "zhcn": return "zhcn";
                case "zhtw": return "zhtw";
                case "fr": return "fr";
                case "it": return "it";
                case "de": return "de";
                case "es": return "es";
                case "en": return "en";
            }

            return "en"; // fallback
        }
    }

    // -------------------------
    // English (default)
    // -------------------------
    private static readonly Dictionary<string, string> EN = new Dictionary<string, string>
    {
        // Interface messages
        { "ControllerAssigned", "Controller {0} is now assigned to character {1}." },
        { "CharacterUnassigned", "Character {0} is now unassigned." },
        { "SelectControllerFirst", "Select a controller first to clear." },
        { "SelectedController", "Selected controller {0}." },
        { "CharacterAssignedToController", "Character {0} is assigned to controller {1}." },
        { "CharacterNotAssigned", "Character {0} is not assigned to any controllers." },
        { "NoControllersDetected", "No controllers detected." },
        { "PartyAIEnabled", "Party AI enabled." },
        { "PartyAIDisabled", "Party AI disabled." },
        { "TargetingHost", "Controller {0} is targeting host's target." },
        { "TargetingClosest", "Controller {0} is targeting their closest target." },

        // Plugin messages
        { "PluginLoaded", "GDM Coop Plugin Loaded" },
        { "PluginAuthors", "Developed by GD Seirya & Mithras Seirya" },
        { "ControllerDisabled", "Controller {0} is disabled." },
        { "InterfaceNull", "GDMModInterface is null. Cannot assign controller." }
    };

    // -------------------------
    // Japanese
    // -------------------------
    private static readonly Dictionary<string, string> JA = new Dictionary<string, string>
    {
        { "ControllerAssigned", "コントローラー {0} がキャラクター {1} に割り当てられました。" },
        { "CharacterUnassigned", "キャラクター {0} の割り当てが解除されました。" },
        { "SelectControllerFirst", "割り当てを解除するには、先にコントローラーを選択してください。" },
        { "SelectedController", "コントローラー {0} を選択しました。" },
        { "CharacterAssignedToController", "キャラクター {0} はコントローラー {1} に割り当てられています。" },
        { "CharacterNotAssigned", "キャラクター {0} はどのコントローラーにも割り当てられていません。" },
        { "NoControllersDetected", "コントローラーが検出されませんでした。" },
        { "PartyAIEnabled", "パーティ AI が有効になりました。" },
        { "PartyAIDisabled", "パーティ AI が無効になりました。" },
        { "TargetingHost", "コントローラー {0} はホストのターゲットを狙っています。" },
        { "TargetingClosest", "コントローラー {0} は最も近いターゲットを狙っています。" },

        { "PluginLoaded", "GDM Coop Plugin が読み込まれました" },
        { "PluginAuthors", "GD Seirya と Mithras Seirya によって開発されました" },
        { "ControllerDisabled", "コントローラー {0} は無効化されています。" },
        { "InterfaceNull", "GDMModInterface が null のため、コントローラーを割り当てできません。" }
    };

    // -------------------------
    // Korean
    // -------------------------
    private static readonly Dictionary<string, string> KO = new Dictionary<string, string>
    {
        { "ControllerAssigned", "컨트롤러 {0}가 캐릭터 {1}에 배정되었습니다." },
        { "CharacterUnassigned", "캐릭터 {0}의 배정이 해제되었습니다." },
        { "SelectControllerFirst", "먼저 컨트롤러를 선택하세요." },
        { "SelectedController", "컨트롤러 {0}를 선택했습니다." },
        { "CharacterAssignedToController", "캐릭터 {0}는 컨트롤러 {1}에 배정되어 있습니다." },
        { "CharacterNotAssigned", "캐릭터 {0}는 어떤 컨트롤러에도 배정되지 않았습니다." },
        { "NoControllersDetected", "컨트롤러가 감지되지 않았습니다." },
        { "PartyAIEnabled", "파티 AI가 활성화되었습니다." },
        { "PartyAIDisabled", "파티 AI가 비활성화되었습니다." },
        { "TargetingHost", "컨트롤러 {0}가 호스트가 선택한 대상을 공격 대상으로 지정합니다." },
        { "TargetingClosest", "컨트롤러 {0}가 가장 가까운 타겟을 조준합니다." },

        { "PluginLoaded", "GDM Coop Plugin 로드됨" },
        { "PluginAuthors", "GD Seirya & Mithras Seirya 개발" },
        { "ControllerDisabled", "컨트롤러 {0}은(는) 비활성화되었습니다." },
        { "InterfaceNull", "GDMModInterface가 null입니다. 컨트롤러를 배정할 수 없습니다." }
    };

    // -------------------------
    // Simplified Chinese (zh-CN)
    // -------------------------
    private static readonly Dictionary<string, string> ZHCN = new Dictionary<string, string>
    {
        { "ControllerAssigned", "控制器 {0} 已分配给角色 {1}。" },
        { "CharacterUnassigned", "角色 {0} 的分配已取消。" },
        { "SelectControllerFirst", "请先选择一个控制器。" },
        { "SelectedController", "已选择控制器 {0}。" },
        { "CharacterAssignedToController", "角色 {0} 已分配给控制器 {1}。" },
        { "CharacterNotAssigned", "角色 {0} 未分配给任何控制器。" },
        { "NoControllersDetected", "未检测到控制器。" },
        { "PartyAIEnabled", "队伍 AI 已启用。" },
        { "PartyAIDisabled", "队伍 AI 已禁用。" },
        { "TargetingHost", "控制器 {0} 正在瞄准主机的目标。" },
        { "TargetingClosest", "控制器 {0} 正在瞄准最近的目标。" },

        { "PluginLoaded", "GDM Coop 插件已加载" },
        { "PluginAuthors", "由 GD Seirya 和 Mithras Seirya 开发" },
        { "ControllerDisabled", "控制器 {0} 已禁用。" },
        { "InterfaceNull", "GDMModInterface 为 null，无法分配控制器。" }
    };

    // -------------------------
    // Traditional Chinese (zh-TW)
    // -------------------------
    private static readonly Dictionary<string, string> ZHTW = new Dictionary<string, string>
    {
        { "ControllerAssigned", "控制器 {0} 已分配給角色 {1}。" },
        { "CharacterUnassigned", "角色 {0} 的分配已取消。" },
        { "SelectControllerFirst", "請先選擇控制器。" },
        { "SelectedController", "已選擇控制器 {0}。" },
        { "CharacterAssignedToController", "角色 {0} 已分配給控制器 {1}。" },
        { "CharacterNotAssigned", "角色 {0} 未分配給任何控制器。" },
        { "NoControllersDetected", "未偵測到控制器。" },
        { "PartyAIEnabled", "隊伍 AI 已啟用。" },
        { "PartyAIDisabled", "隊伍 AI 已停用。" },
        { "TargetingHost", "控制器 {0} 正在瞄準主機的目標。" },
        { "TargetingClosest", "控制器 {0} 正在瞄準最近的目標。" },

        { "PluginLoaded", "GDM Coop 插件已載入" },
        { "PluginAuthors", "由 GD Seirya 與 Mithras Seirya 開發" },
        { "ControllerDisabled", "控制器 {0} 已停用。" },
        { "InterfaceNull", "GDMModInterface 為 null，無法分配控制器。" }
    };

    // -------------------------
    // French
    // -------------------------
    private static readonly Dictionary<string, string> FR = new Dictionary<string, string>
    {
        { "ControllerAssigned", "Le contrôleur {0} est maintenant assigné au personnage {1}." },
        { "CharacterUnassigned", "Le personnage {0} n'est plus assigné." },
        { "SelectControllerFirst", "Veuillez d'abord sélectionner un contrôleur." },
        { "SelectedController", "Contrôleur {0} sélectionné." },
        { "CharacterAssignedToController", "Le personnage {0} est assigné au contrôleur {1}." },
        { "CharacterNotAssigned", "Le personnage {0} n'est assigné à aucun contrôleur." },
        { "NoControllersDetected", "Aucun contrôleur détecté." },
        { "PartyAIEnabled", "IA de l'équipe activée." },
        { "PartyAIDisabled", "IA de l'équipe désactivée." },
        { "TargetingHost", "Le contrôleur {0} vise la cible de l'hôte." },
        { "TargetingClosest", "Le contrôleur {0} vise la cible la plus proche." },

        { "PluginLoaded", "GDM Coop Plugin chargé" },
        { "PluginAuthors", "Développé par GD Seirya & Mithras Seirya" },
        { "ControllerDisabled", "Le contrôleur {0} est désactivé." },
        { "InterfaceNull", "GDMModInterface est nul. Impossible d'assigner le contrôleur." }
    };

    // -------------------------
    // Italian
    // -------------------------
    private static readonly Dictionary<string, string> IT = new Dictionary<string, string>
    {
        { "ControllerAssigned", "Il controller {0} è stato assegnato al personaggio {1}." },
        { "CharacterUnassigned", "Il personaggio {0} non è più assegnato." },
        { "SelectControllerFirst", "Seleziona prima un controller." },
        { "SelectedController", "Controller {0} selezionato." },
        { "CharacterAssignedToController", "Il personaggio {0} è assegnato al controller {1}." },
        { "CharacterNotAssigned", "Il personaggio {0} non è assegnato a nessun controller." },
        { "NoControllersDetected", "Nessun controller rilevato." },
        { "PartyAIEnabled", "IA della squadra attivata." },
        { "PartyAIDisabled", "IA della squadra disattivata." },
        { "TargetingHost", "Il controller {0} sta mirando al bersaglio dell'host." },
        { "TargetingClosest", "Il controller {0} sta mirando al bersaglio più vicino." },

        { "PluginLoaded", "GDM Coop Plugin caricato" },
        { "PluginAuthors", "Sviluppato da GD Seirya & Mithras Seirya" },
        { "ControllerDisabled", "Il controller {0} è disattivato." },
        { "InterfaceNull", "GDMModInterface è nullo. Impossibile assegnare il controller." }
    };

    // -------------------------
    // German
    // -------------------------
    private static readonly Dictionary<string, string> DE = new Dictionary<string, string>
    {
        { "ControllerAssigned", "Controller {0} wurde Charakter {1} zugewiesen." },
        { "CharacterUnassigned", "Charakter {0} ist nicht mehr zugewiesen." },
        { "SelectControllerFirst", "Bitte zuerst einen Controller auswählen." },
        { "SelectedController", "Controller {0} ausgewählt." },
        { "CharacterAssignedToController", "Charakter {0} ist Controller {1} zugewiesen." },
        { "CharacterNotAssigned", "Charakter {0} ist keinem Controller zugewiesen." },
        { "NoControllersDetected", "Keine Controller erkannt." },
        { "PartyAIEnabled", "Gruppen‑KI aktiviert." },
        { "PartyAIDisabled", "Gruppen‑KI deaktiviert." },
        { "TargetingHost", "Controller {0} greift das Ziel des Hosts an." },
        { "TargetingClosest", "Controller {0} zielt auf das nächste Ziel." },

        { "PluginLoaded", "GDM Coop Plugin geladen" },
        { "PluginAuthors", "Entwickelt von GD Seirya & Mithras Seirya" },
        { "ControllerDisabled", "Controller {0} ist deaktiviert." },
        { "InterfaceNull", "GDMModInterface ist null. Controller kann nicht zugewiesen werden." }
    };

    // -------------------------
    // Spanish
    // -------------------------
    private static readonly Dictionary<string, string> ES = new Dictionary<string, string>
    {
        { "ControllerAssigned", "El controlador {0} ahora está asignado al personaje {1}." },
        { "CharacterUnassigned", "El personaje {0} ya no está asignado." },
        { "SelectControllerFirst", "Primero selecciona un controlador." },
        { "SelectedController", "Controlador {0} seleccionado." },
        { "CharacterAssignedToController", "El personaje {0} está asignado al controlador {1}." },
        { "CharacterNotAssigned", "El personaje {0} no está asignado a ningún controlador." },
        { "NoControllersDetected", "No se detectaron controladores." },
        { "PartyAIEnabled", "IA del grupo activada." },
        { "PartyAIDisabled", "IA del grupo desactivada." },
        { "TargetingHost", "El controlador {0} apunta al objetivo del anfitrión." },
        { "TargetingClosest", "El controlador {0} apunta al objetivo más cercano." },

        { "PluginLoaded", "GDM Coop Plugin cargado" },
        { "PluginAuthors", "Desarrollado por GD Seirya & Mithras Seirya" },
        { "ControllerDisabled", "El controlador {0} está desactivado." },
        { "InterfaceNull", "GDMModInterface es nulo. No se puede asignar el controlador." }
    };

    /// <summary>
    /// Returns string based on set language
    /// Example: Entering "ControllerAssigned" key followed by arguments fills in the remaining parameters in curly bracers.
    /// </summary>
    public static string Get(string key, params object[] args)
    {
        Dictionary<string, string> table = EN;

        switch (CurrentLang)
        {
            case "ja": table = JA; break;
            case "ko": table = KO; break;
            case "zhcn": table = ZHCN; break;
            case "zhtw": table = ZHTW; break;
            case "fr": table = FR; break;
            case "it": table = IT; break;
            case "de": table = DE; break;
            case "es": table = ES; break;
        }

        if (table.TryGetValue(key, out string value))
            return string.Format(value, args);

        return key;
    }
}