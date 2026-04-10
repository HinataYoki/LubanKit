#if UNITY_EDITOR && YOKIFRAME_LUBAN_SUPPORT
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YokiFrame.TableKit.Editor
{
    public partial class TableKitEditorUI
    {
        private enum UILanguage
        {
            Chinese,
            English,
        }

        private const string PREF_UI_LANGUAGE = "TableKit_UILanguage";

        private static readonly Dictionary<string, string> ZH = new()
        {
            ["lang.button"] = "中 / EN",
            ["lang.tooltip"] = "切换界面语言 / Switch UI language",
            ["command.title"] = "TableKit 配置表生成",
            ["command.reset"] = "还原默认",
            ["command.reset.tooltip"] = "还原所有配置为默认值",
            ["command.open"] = "打开配置表",
            ["command.open.tooltip"] = "打开 Luban 配置表数据目录 (Datas)",
            ["command.generate"] = "生成配置表",
            ["config.title"] = "环境与路径配置",
            ["config.warning"] = "Luban 工具不应放置在 Assets 内部，推荐放置在与 Assets 同级目录",
            ["config.luban"] = "Luban 环境",
            ["config.workdir"] = "工作目录:",
            ["config.workdir.hint"] = "包含 Datas、Defines、luban.conf 的文件夹",
            ["config.workdir.select"] = "选择 Luban 工作目录",
            ["config.dll"] = "Luban.dll:",
            ["config.dll.hint"] = "Luban 代码生成工具的 DLL 路径",
            ["config.dll.select"] = "选择 Luban.dll 文件",
            ["config.output"] = "输出路径",
            ["config.data.target"] = "数据格式:",
            ["config.data.output"] = "数据输出:",
            ["config.data.output.hint"] = "生成的配置数据文件存放路径，默认 Assets/Resources/Art/Table/",
            ["config.data.output.select"] = "选择数据输出目录",
            ["config.code.output"] = "代码输出:",
            ["config.code.output.hint"] = "生成的 C# 配置表代码存放路径",
            ["config.code.output.select"] = "选择代码输出目录",
            ["config.tablekit"] = "TableKit 路径",
            ["config.custom.editor"] = "自定义编辑器数据路径",
            ["config.custom.editor.hint"] = "关闭时编辑器数据路径自动跟随数据输出路径",
            ["config.editor.data"] = "编辑器数据:",
            ["config.editor.data.hint"] = "TableKit.TablesEditor 编辑器访问用的表数据路径",
            ["config.editor.data.select"] = "选择编辑器数据路径",
            ["config.runtime.mode"] = "运行时模式:",
            ["config.runtime.hint"] = "{0} 为文件名占位符 • 可寻址模式填 {0} • 完整路径填 Assets/Art/Table/{0}",
            ["build.title"] = "构建选项",
            ["build.asm"] = "使用独立程序集",
            ["build.asm.name"] = "程序集名称:",
            ["build.asm.hint"] = "打开后生成的代码会放入独立程序集 (asmdef)",
            ["build.external"] = "生成 ExternalTypeUtil",
            ["build.external.hint"] = "Luban vector 转 Unity Vector，如有需要可自行添加代码，不会重复生成覆盖",
            ["build.external.warn"] = "注意：TableKit.cs 会被重复生成覆盖，请勿在其中添加自定义代码",
            ["build.async"] = "异步加载模式",
            ["build.async.hint"] = "启用后生成 InitAsync() 方法，使用 UniTask 异步加载配置表数据",
            ["build.async.warn"] = "需要项目已安装 UniTask 并定义 YOKIFRAME_UNITASK_SUPPORT",
            ["browse.folder"] = "浏览文件夹",
            ["browse.file"] = "浏览文件",
            ["open.finder"] = "在资源管理器中打开",
            ["dialog.info"] = "提示",
            ["dialog.error"] = "配置错误",
            ["dialog.ok"] = "确定",
            ["dialog.cancel"] = "取消",
            ["dialog.reset.title"] = "还原默认设置",
            ["dialog.reset.message"] = "确定要将所有配置还原为默认值吗？",
            ["dialog.dir.missing"] = "目录不存在:\n{0}",
            ["dialog.workdir.invalid"] = "Luban 工作目录不存在\n路径: {0}",
            ["dialog.conf.missing"] = "找不到 luban.conf 文件\n路径: {0}",
            ["dialog.dll.invalid"] = "Luban.dll 路径无效\n路径: {0}",
            ["dialog.workdir.unset"] = "Luban 工作目录未配置或不存在\n路径: {0}",
            ["log.reset"] = "[{0:HH:mm:ss}] 已还原为默认设置",
            ["extra.title"] = "额外输出目标",
            ["extra.hint"] = "可添加多个输出目标，每个目标可独立选择导出字段分组",
            ["extra.warn"] = "提示: 不同导出目标会分批运行 Luban，确保字段正确导出",
            ["extra.add"] = "+ 添加输出目标",
            ["extra.target.tooltip"] = "决定导出哪些字段分组（client=客户端字段, server=服务端字段, all=全部）",
            ["extra.generate"] = "生成",
            ["extra.generate.tooltip"] = "仅生成此目标的数据和代码",
            ["extra.data"] = "数据:",
            ["extra.code"] = "代码:",
            ["extra.select.data"] = "选择数据输出目录",
            ["extra.select.code"] = "选择代码输出目录",
            ["extra.disabled"] = "此目标已禁用",
            ["preview.title"] = "数据预览",
            ["preview.validate"] = "验证配置",
            ["preview.hint"] = "点击「验证配置」后显示数据预览",
            ["preview.dir.missing"] = "验证数据目录不存在",
            ["preview.no.json"] = "没有找到 JSON 数据文件",
            ["preview.select"] = "选择配置表:",
            ["preview.match"] = "找到 {0} 处匹配",
            ["preview.no.match"] = "无匹配",
            ["preview.json.fail"] = "JSON 解析失败",
            ["preview.load.fail"] = "加载失败: {0}",
            ["tables.title"] = "配置表信息",
            ["tables.refresh"] = "刷新缓存",
            ["tables.search"] = "[搜索]",
            ["tables.placeholder"] = "搜索表名...",
            ["tables.hint"] = "点击「刷新缓存」加载配置表信息",
            ["tables.not.loaded"] = "配置表未加载",
            ["tables.stats.total"] = "共 {0} 个配置表",
            ["tables.stats.match"] = "找到 {0}/{1} 个匹配项",
            ["tables.no.match"] = "没有找到匹配的配置表",
            ["guide.title"] = "使用指南",
            ["guide.basic.title"] = "基础用法 (Resources 加载)",
            ["guide.basic.desc"] = "TableKit 默认使用 Resources.Load 加载配置数据，无需额外配置：",
            ["guide.custom.title"] = "自定义加载器",
            ["guide.custom.desc"] = "如需使用 Addressables 或 YooAsset 等资源管理方案，可实现自定义加载器：",
            ["guide.yoo.title"] = "YooAsset 加载器示例",
            ["guide.yoo.desc"] = "使用 YooAsset 加载配置表的完整实现：",
            ["guide.async.title"] = "异步加载模式",
            ["guide.async.desc"] = "开启构建选项中的「异步加载模式」后，生成的代码包含 InitAsync 异步初始化方法。可通过 SetAsyncBinaryLoader/SetAsyncJsonLoader 自定义异步加载方式，通过 SetTableFileNames 覆盖预加载的文件列表。如果不显式调用 InitAsync，首次访问 TableKit.Tables 时将自动触发同步 Init() 加载：",
            ["guide.note.title"] = "注意事项",
            ["console.title"] = "控制台",
            ["console.clear"] = "清除",
            ["console.ready"] = "就绪",
            ["console.building"] = "生成中...",
            ["console.success"] = "生成成功",
            ["console.failed"] = "生成失败",
            ["console.cleared"] = "日志已清除",
            ["console.waiting"] = "等待操作...",
            ["log.start"] = "[{0:HH:mm:ss}] 开始{1}...",
            ["log.action.validate"] = "验证",
            ["log.action.generate"] = "生成",
            ["log.refresh.db"] = "\n[OK] 已刷新 Unity 资源数据库",
            ["log.exception"] = "[异常] {0}",
            ["log.exit.code"] = "[{0:HH:mm:ss}] 退出码: {1}",
            ["log.single.target"] = "[{0:HH:mm:ss}] 单独生成目标: {1}",
            ["log.batch.failed"] = "[批次 {0}] 生成失败，停止后续批次",
            ["log.gen.runtime"] = "正在生成 TableKit 运行时代码...",
            ["log.gen.runtime.done"] = "[OK] TableKit 运行时代码生成完成",
            ["log.gen.async.done"] = "[OK] 已生成异步加载代码 (InitAsync/ReloadAsync)",
            ["log.gen.external.done"] = "[OK] 已生成 ExternalTypeUtil.cs",
            ["log.editor.cache.refreshed"] = "[{0:HH:mm:ss}] [OK] 编辑器缓存已刷新",
            ["log.editor.load.failed"] = "[{0:HH:mm:ss}] [FAIL] 加载配置表失败:\n{1}",
            ["log.preview.files"] = "[OK] 找到 {0} 个数据文件",
        };

        private static readonly Dictionary<string, string> EN = new()
        {
            ["lang.button"] = "中 / EN",
            ["lang.tooltip"] = "Switch UI language / 切换界面语言",
            ["command.title"] = "TableKit Config Generation",
            ["command.reset"] = "Reset Defaults",
            ["command.reset.tooltip"] = "Restore all settings to their default values",
            ["command.open"] = "Open Tables",
            ["command.open.tooltip"] = "Open the Luban table data directory (Datas)",
            ["command.generate"] = "Generate Tables",
            ["config.title"] = "Environment & Paths",
            ["config.warning"] = "Do not place Luban tools inside Assets. Keeping them next to Assets is recommended.",
            ["config.luban"] = "Luban Environment",
            ["config.workdir"] = "Work Dir:",
            ["config.workdir.hint"] = "Folder containing Datas, Defines, and luban.conf",
            ["config.workdir.select"] = "Select Luban work directory",
            ["config.dll"] = "Luban.dll:",
            ["config.dll.hint"] = "DLL path of the Luban code generation tool",
            ["config.dll.select"] = "Select Luban.dll",
            ["config.output"] = "Output Paths",
            ["config.data.target"] = "Data Format:",
            ["config.data.output"] = "Data Output:",
            ["config.data.output.hint"] = "Output path for generated config data, default is Assets/Resources/Art/Table/",
            ["config.data.output.select"] = "Select data output directory",
            ["config.code.output"] = "Code Output:",
            ["config.code.output.hint"] = "Output path for generated C# table code",
            ["config.code.output.select"] = "Select code output directory",
            ["config.tablekit"] = "TableKit Paths",
            ["config.custom.editor"] = "Custom Editor Data Path",
            ["config.custom.editor.hint"] = "When disabled, the editor data path follows the data output path",
            ["config.editor.data"] = "Editor Data:",
            ["config.editor.data.hint"] = "Table data path used by TableKit.TablesEditor",
            ["config.editor.data.select"] = "Select editor data path",
            ["config.runtime.mode"] = "Runtime Path:",
            ["config.runtime.hint"] = "{0} is the filename placeholder • use {0} for addressable paths • use Assets/Art/Table/{0} for full paths",
            ["build.title"] = "Build Options",
            ["build.asm"] = "Use Assembly Definition",
            ["build.asm.name"] = "Assembly Name:",
            ["build.asm.hint"] = "Generated code will be placed into a separate assembly (asmdef)",
            ["build.external"] = "Generate ExternalTypeUtil",
            ["build.external.hint"] = "Converts Luban vectors to Unity vectors. Safe to extend outside generated files.",
            ["build.external.warn"] = "Warning: TableKit.cs will be regenerated. Do not add custom code inside it.",
            ["build.async"] = "Async Loading Mode",
            ["build.async.hint"] = "Generates InitAsync() and loads table data asynchronously with UniTask",
            ["build.async.warn"] = "UniTask must be installed and YOKIFRAME_UNITASK_SUPPORT must be defined",
            ["browse.folder"] = "Browse folder",
            ["browse.file"] = "Browse file",
            ["open.finder"] = "Open in file explorer",
            ["dialog.info"] = "Info",
            ["dialog.error"] = "Configuration Error",
            ["dialog.ok"] = "OK",
            ["dialog.cancel"] = "Cancel",
            ["dialog.reset.title"] = "Restore Defaults",
            ["dialog.reset.message"] = "Are you sure you want to restore all settings to their default values?",
            ["dialog.dir.missing"] = "Directory does not exist:\n{0}",
            ["dialog.workdir.invalid"] = "Luban work directory does not exist\nPath: {0}",
            ["dialog.conf.missing"] = "Could not find luban.conf\nPath: {0}",
            ["dialog.dll.invalid"] = "Invalid Luban.dll path\nPath: {0}",
            ["dialog.workdir.unset"] = "Luban work directory is not configured or does not exist\nPath: {0}",
            ["log.reset"] = "[{0:HH:mm:ss}] Restored default settings",
            ["extra.title"] = "Extra Output Targets",
            ["extra.hint"] = "Add multiple output targets, each with its own field group",
            ["extra.warn"] = "Tip: Different output targets run Luban in batches to ensure correct field export",
            ["extra.add"] = "+ Add Output Target",
            ["extra.target.tooltip"] = "Choose which field group to export (client, server, or all)",
            ["extra.generate"] = "Generate",
            ["extra.generate.tooltip"] = "Generate data and code only for this target",
            ["extra.data"] = "Data:",
            ["extra.code"] = "Code:",
            ["extra.select.data"] = "Select data output directory",
            ["extra.select.code"] = "Select code output directory",
            ["extra.disabled"] = "This target is disabled",
            ["preview.title"] = "Data Preview",
            ["preview.validate"] = "Validate Config",
            ["preview.hint"] = "Click \"Validate Config\" to preview generated data",
            ["preview.dir.missing"] = "Validated data directory does not exist",
            ["preview.no.json"] = "No JSON data files found",
            ["preview.select"] = "Select Table:",
            ["preview.match"] = "{0} matches found",
            ["preview.no.match"] = "No matches",
            ["preview.json.fail"] = "Failed to parse JSON",
            ["preview.load.fail"] = "Load failed: {0}",
            ["tables.title"] = "Table Info",
            ["tables.refresh"] = "Refresh Cache",
            ["tables.search"] = "[Search]",
            ["tables.placeholder"] = "Search table name...",
            ["tables.hint"] = "Click \"Refresh Cache\" to load table info",
            ["tables.not.loaded"] = "Tables are not loaded",
            ["tables.stats.total"] = "{0} tables in total",
            ["tables.stats.match"] = "{0}/{1} matches",
            ["tables.no.match"] = "No matching tables found",
            ["guide.title"] = "Usage Guide",
            ["guide.basic.title"] = "Basic Usage (Resources)",
            ["guide.basic.desc"] = "By default, TableKit loads config data with Resources.Load and needs no extra setup:",
            ["guide.custom.title"] = "Custom Loader",
            ["guide.custom.desc"] = "If you use Addressables, YooAsset, or another resource pipeline, implement a custom loader:",
            ["guide.yoo.title"] = "YooAsset Loader Example",
            ["guide.yoo.desc"] = "A complete example for loading config tables with YooAsset:",
            ["guide.async.title"] = "Async Loading Mode",
            ["guide.async.desc"] = "When \"Async Loading Mode\" is enabled in Build Options, generated code includes InitAsync() for asynchronous initialization. You can customize async loading with SetAsyncBinaryLoader/SetAsyncJsonLoader and override the preload file list with SetTableFileNames. If InitAsync is not called explicitly, the first access to TableKit.Tables will fall back to synchronous Init():",
            ["guide.note.title"] = "Notes",
            ["console.title"] = "Console",
            ["console.clear"] = "Clear",
            ["console.ready"] = "Ready",
            ["console.building"] = "Generating...",
            ["console.success"] = "Succeeded",
            ["console.failed"] = "Failed",
            ["console.cleared"] = "Log cleared",
            ["console.waiting"] = "Waiting for action...",
            ["log.start"] = "[{0:HH:mm:ss}] Start {1}...",
            ["log.action.validate"] = "validation",
            ["log.action.generate"] = "generation",
            ["log.refresh.db"] = "\n[OK] Unity AssetDatabase refreshed",
            ["log.exception"] = "[Exception] {0}",
            ["log.exit.code"] = "[{0:HH:mm:ss}] Exit code: {1}",
            ["log.single.target"] = "[{0:HH:mm:ss}] Generate single target: {1}",
            ["log.batch.failed"] = "[Batch {0}] Generation failed, stopping remaining batches",
            ["log.gen.runtime"] = "Generating TableKit runtime code...",
            ["log.gen.runtime.done"] = "[OK] TableKit runtime code generated",
            ["log.gen.async.done"] = "[OK] Async loading code generated (InitAsync/ReloadAsync)",
            ["log.gen.external.done"] = "[OK] ExternalTypeUtil.cs generated",
            ["log.editor.cache.refreshed"] = "[{0:HH:mm:ss}] [OK] Editor cache refreshed",
            ["log.editor.load.failed"] = "[{0:HH:mm:ss}] [FAIL] Failed to load config tables:\n{1}",
            ["log.preview.files"] = "[OK] Found {0} data files",
        };

        private UILanguage mLanguage;
        private readonly Action mRequestRebuild;

        public TableKitEditorUI(Action requestRebuild = null)
        {
            mRequestRebuild = requestRebuild;
        }

        private void LoadLanguage()
        {
            var stored = EditorPrefs.GetString(PREF_UI_LANGUAGE, string.Empty);
            if (stored == "en")
            {
                mLanguage = UILanguage.English;
                return;
            }

            if (stored == "zh")
            {
                mLanguage = UILanguage.Chinese;
                return;
            }

            mLanguage = Application.systemLanguage is SystemLanguage.ChineseSimplified or SystemLanguage.ChineseTraditional
                ? UILanguage.Chinese
                : UILanguage.English;
        }

        private void SaveLanguage() => EditorPrefs.SetString(PREF_UI_LANGUAGE, mLanguage == UILanguage.Chinese ? "zh" : "en");

        private string T(string key)
        {
            var map = mLanguage == UILanguage.Chinese ? ZH : EN;
            return map.TryGetValue(key, out var value) ? value : key;
        }

        private string TF(string key, params object[] args) => string.Format(T(key), args);

        private void ToggleLanguage()
        {
            mLanguage = mLanguage == UILanguage.Chinese ? UILanguage.English : UILanguage.Chinese;
            SaveLanguage();
            mRequestRebuild?.Invoke();
        }
    }
}
#endif
