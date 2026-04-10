#if UNITY_EDITOR && YOKIFRAME_LUBAN_SUPPORT
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace YokiFrame.TableKit.Editor
{
    /// <summary>
    /// TableKitEditorUI - 配置内容区块
    /// </summary>
    public partial class TableKitEditorUI
    {
        #region 可折叠配置区

        private VisualElement BuildConfigFoldout()
        {
            var container = new VisualElement();
            container.style.backgroundColor = new StyleColor(Design.LayerCard);
            container.style.borderTopLeftRadius = container.style.borderTopRightRadius = 8;
            container.style.borderBottomLeftRadius = container.style.borderBottomRightRadius = 8;
            container.style.borderLeftWidth = container.style.borderRightWidth = 1;
            container.style.borderTopWidth = container.style.borderBottomWidth = 1;
            container.style.borderLeftColor = container.style.borderRightColor = new StyleColor(Design.BorderDefault);
            container.style.borderTopColor = container.style.borderBottomColor = new StyleColor(Design.BorderDefault);
            container.style.marginBottom = 12;
            container.style.overflow = Overflow.Hidden;

            // 折叠头部
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.alignItems = Align.Center;
            header.style.paddingLeft = 12;
            header.style.paddingRight = 12;
            header.style.paddingTop = 10;
            header.style.paddingBottom = 10;
            header.style.cursor = StyleKeyword.Initial;
            container.Add(header);

            var arrow = new Image { name = "foldout-arrow", image = TableKitIcons.GetIcon(TableKitIcons.CHEVRON_RIGHT) };
            arrow.style.width = 12;
            arrow.style.height = 12;
            arrow.style.marginRight = 6;
            header.Add(arrow);

            var title = new Label(T("config.title"));
            title.style.fontSize = Design.FontSizeSection;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = new StyleColor(Design.TextPrimary);
            title.style.flexGrow = 1;
            header.Add(title);

            // 状态点
            mConfigStatusDot = new VisualElement();
            mConfigStatusDot.style.width = 8;
            mConfigStatusDot.style.height = 8;
            mConfigStatusDot.style.borderTopLeftRadius = mConfigStatusDot.style.borderTopRightRadius = 4;
            mConfigStatusDot.style.borderBottomLeftRadius = mConfigStatusDot.style.borderBottomRightRadius = 4;
            mConfigStatusDot.style.backgroundColor = new StyleColor(Design.BrandSuccess);
            header.Add(mConfigStatusDot);

            // 折叠内容
            bool isExpanded = EditorPrefs.GetBool(PREF_CONFIG_EXPANDED, false);
            mConfigFoldout = new VisualElement();
            mConfigFoldout.style.paddingLeft = 12;
            mConfigFoldout.style.paddingRight = 12;
            mConfigFoldout.style.paddingBottom = 12;
            mConfigFoldout.style.borderTopWidth = 1;
            mConfigFoldout.style.borderTopColor = new StyleColor(Design.BorderDefault);
            mConfigFoldout.style.display = isExpanded ? DisplayStyle.Flex : DisplayStyle.None;
            container.Add(mConfigFoldout);

            arrow.image = isExpanded ? TableKitIcons.GetIcon(TableKitIcons.CHEVRON_DOWN) : TableKitIcons.GetIcon(TableKitIcons.CHEVRON_RIGHT);

            header.RegisterCallback<ClickEvent>(_ =>
            {
                bool expanded = mConfigFoldout.style.display == DisplayStyle.Flex;
                mConfigFoldout.style.display = expanded ? DisplayStyle.None : DisplayStyle.Flex;
                arrow.image = expanded ? TableKitIcons.GetIcon(TableKitIcons.CHEVRON_RIGHT) : TableKitIcons.GetIcon(TableKitIcons.CHEVRON_DOWN);
                EditorPrefs.SetBool(PREF_CONFIG_EXPANDED, !expanded);
            });

            BuildConfigContent(mConfigFoldout);
            return container;
        }

        private void BuildConfigContent(VisualElement container)
        {
            // 警告 Callout
            var warning = CreateCallout(T("config.warning"), Design.BrandWarning);
            warning.style.marginTop = 12;
            container.Add(warning);

            BuildLubanSection(container);
            BuildOutputSection(container);
            BuildTableKitSection(container);
        }

        private void BuildLubanSection(VisualElement container)
        {
            var lubanSection = CreateSubSection(T("config.luban"));
            container.Add(lubanSection);

            lubanSection.Add(CreateValidatedPathRow(T("config.workdir"), ref mLubanWorkDirField, mLubanWorkDir, path =>
            {
                mLubanWorkDir = path;
                mLubanWorkDirField.value = path;
                SavePrefs();
                RefreshConfigStatus();
            }, true, T("config.workdir.select")));

            var workDirHint = new Label(T("config.workdir.hint"));
            workDirHint.style.fontSize = Design.FontSizeSmall;
            workDirHint.style.color = new StyleColor(Design.TextTertiary);
            workDirHint.style.marginTop = 2;
            workDirHint.style.marginLeft = 80;
            lubanSection.Add(workDirHint);

            lubanSection.Add(CreateValidatedFileRow(T("config.dll"), ref mLubanDllPathField, mLubanDllPath, path =>
            {
                mLubanDllPath = path;
                mLubanDllPathField.value = path;
                SavePrefs();
                RefreshConfigStatus();
            }, "dll", T("config.dll.select")));

            var dllHint = new Label(T("config.dll.hint"));
            dllHint.style.fontSize = Design.FontSizeSmall;
            dllHint.style.color = new StyleColor(Design.TextTertiary);
            dllHint.style.marginTop = 2;
            dllHint.style.marginLeft = 80;
            lubanSection.Add(dllHint);
        }

        private void BuildOutputSection(VisualElement container)
        {
            var outputSection = CreateSubSection(T("config.output"));
            container.Add(outputSection);

            // Data Target
            var dataRow = new VisualElement();
            dataRow.style.flexDirection = FlexDirection.Row;
            dataRow.style.alignItems = Align.Center;
            dataRow.style.marginTop = 8;
            outputSection.Add(dataRow);

            var dataLabel = new Label(T("config.data.target"));
            dataLabel.style.width = 80;
            dataLabel.style.color = new StyleColor(Design.TextSecondary);
            dataRow.Add(dataLabel);

            mDataTargetDropdown = new DropdownField(new List<string>(DATA_TARGET_OPTIONS), 0);
            mDataTargetDropdown.style.width = 120;
            mDataTargetDropdown.value = string.IsNullOrEmpty(mDataTarget) ? DATA_TARGET_OPTIONS[0] : mDataTarget;
            mDataTargetDropdown.RegisterValueChangedCallback(evt =>
            {
                mDataTarget = evt.newValue;
                var newCodeTarget = evt.newValue == "bin" ? "cs-bin" : "cs-simple-json";
                if (mCodeTarget != newCodeTarget)
                {
                    mCodeTarget = newCodeTarget;
                    mCodeTargetDropdown?.SetValueWithoutNotify(newCodeTarget);
                }
                SavePrefs();
            });
            dataRow.Add(mDataTargetDropdown);

            outputSection.Add(CreateValidatedPathRow(T("config.data.output"), ref mOutputDataDirField, mOutputDataDir, path =>
            {
                mOutputDataDir = path;
                mOutputDataDirField.value = path;
                if (!mCustomEditorDataPath)
                {
                    mEditorDataPath = path;
                    mEditorDataPathField?.SetValueWithoutNotify(path);
                }
                SavePrefs();
                RefreshConfigStatus();
            }, false, T("config.data.output.select")));

            var dataOutputHint = new Label(T("config.data.output.hint"));
            dataOutputHint.style.fontSize = Design.FontSizeSmall;
            dataOutputHint.style.color = new StyleColor(Design.TextTertiary);
            dataOutputHint.style.marginTop = 2;
            dataOutputHint.style.marginLeft = 80;
            outputSection.Add(dataOutputHint);

            outputSection.Add(CreateValidatedPathRow(T("config.code.output"), ref mOutputCodeDirField, mOutputCodeDir, path =>
            {
                mOutputCodeDir = path;
                mOutputCodeDirField.value = path;
                SavePrefs();
                RefreshConfigStatus();
            }, false, T("config.code.output.select")));

            var codeOutputHint = new Label(T("config.code.output.hint"));
            codeOutputHint.style.fontSize = Design.FontSizeSmall;
            codeOutputHint.style.color = new StyleColor(Design.TextTertiary);
            codeOutputHint.style.marginTop = 2;
            codeOutputHint.style.marginLeft = 80;
            outputSection.Add(codeOutputHint);

            // 多目标输出配置
            BuildExtraOutputSection(container);
        }

        private void BuildTableKitSection(VisualElement container)
        {
            var tkSection = CreateSubSection(T("config.tablekit"));
            container.Add(tkSection);

            // 自定义编辑器数据路径 Toggle
            var customEditorPathRow = new VisualElement();
            customEditorPathRow.style.flexDirection = FlexDirection.Row;
            customEditorPathRow.style.alignItems = Align.Center;
            customEditorPathRow.style.marginTop = 8;
            customEditorPathRow.style.marginBottom = 4;
            tkSection.Add(customEditorPathRow);

            mCustomEditorDataPathToggle = CreateCapsuleToggle(T("config.custom.editor"), mCustomEditorDataPath, v =>
            {
                mCustomEditorDataPath = v;
                mEditorDataPathRow?.SetEnabled(v);
                if (!v)
                {
                    mEditorDataPath = mOutputDataDir;
                    mEditorDataPathField?.SetValueWithoutNotify(mEditorDataPath);
                }
                SavePrefs();
            });
            customEditorPathRow.Add(mCustomEditorDataPathToggle);

            var customEditorPathHint = new Label(T("config.custom.editor.hint"));
            customEditorPathHint.style.fontSize = Design.FontSizeSmall;
            customEditorPathHint.style.color = new StyleColor(Design.TextTertiary);
            customEditorPathHint.style.marginBottom = 8;
            tkSection.Add(customEditorPathHint);

            // 编辑器数据路径
            mEditorDataPathRow = new VisualElement();
            tkSection.Add(mEditorDataPathRow);

            mEditorDataPathRow.Add(CreateValidatedPathRow(T("config.editor.data"), ref mEditorDataPathField, mEditorDataPath, path =>
            {
                mEditorDataPath = path;
                mEditorDataPathField.value = path;
                SavePrefs();
            }, false, T("config.editor.data.select")));

            var editorDataHint = new Label(T("config.editor.data.hint"));
            editorDataHint.style.fontSize = Design.FontSizeSmall;
            editorDataHint.style.color = new StyleColor(Design.TextTertiary);
            editorDataHint.style.marginTop = 2;
            editorDataHint.style.marginLeft = 80;
            mEditorDataPathRow.Add(editorDataHint);

            mEditorDataPathRow.SetEnabled(mCustomEditorDataPath);

            var runtimeRow = new VisualElement();
            runtimeRow.style.flexDirection = FlexDirection.Row;
            runtimeRow.style.alignItems = Align.Center;
            runtimeRow.style.marginTop = 8;
            tkSection.Add(runtimeRow);

            var runtimeLabel = new Label(T("config.runtime.mode"));
            runtimeLabel.style.width = 80;
            runtimeLabel.style.color = new StyleColor(Design.TextSecondary);
            runtimeRow.Add(runtimeLabel);

            var runtimeFieldContainer = new VisualElement();
            runtimeFieldContainer.style.flexDirection = FlexDirection.Row;
            runtimeFieldContainer.style.flexGrow = 1;
            runtimeRow.Add(runtimeFieldContainer);

            mRuntimePathPatternField = new TextField();
            mRuntimePathPatternField.style.flexGrow = 1;
            mRuntimePathPatternField.value = mRuntimePathPattern;
            mRuntimePathPatternField.RegisterValueChangedCallback(evt => { mRuntimePathPattern = evt.newValue; SavePrefs(); });
            runtimeFieldContainer.Add(mRuntimePathPatternField);

            var hint = new Label(T("config.runtime.hint"));
            hint.style.fontSize = Design.FontSizeSmall;
            hint.style.color = new StyleColor(Design.TextTertiary);
            hint.style.marginTop = 4;
            hint.style.marginLeft = 80;
            tkSection.Add(hint);
        }

        #endregion
    }
}
#endif
