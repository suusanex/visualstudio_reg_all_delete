# visualstudio_reg_all_delete
Delete all related registry keys for the purpose of completely uninstalling VisualStudio.

次のプロジェクトを使用してアンインストールを実施した。

https://github.com/Microsoft/VisualStudioUninstaller

しかし、次の問題が発生した。

 * Visual Studio 2015のインストールを実行すると、インストール先パスが前回のもので固定されている
 * そのままインストールを行なったが、一部コンポーネントのインストールが失敗してVisual Studioが正常起動できない。

以前に同様の件でMicrosoftへ問い合わせをしたときに、どうしようも無い時はVisualStudioらしきレジストリキーを全て削除するのが良いと回答をもらった。

そのため、そのようなレジストリ削除を行なうツールを作成した。



# 準備

NuGetで、次のプロジェクトとその依存プロジェクトを取得する。そこに含まれるDLLを、全てCSScriptLauncher.exeと同じフォルダに配置する。

 * Microsoft.CodeAnalysis.Analyzers
 * Microsoft.CodeAnalysis.Common
 * Microsoft.CodeAnalysis.CSharp
 * Microsoft.CodeAnalysis.CSharp.Scripting
 * Microsoft.CodeAnalysis.Scripting.Common


2017/2/13時点では、少なくとも次のファイルを配置すれば実行可能であることを確認済み。

 * Microsoft.CodeAnalysis.CSharp.dll
 * Microsoft.CodeAnalysis.CSharp.Scripting.dll
 * Microsoft.CodeAnalysis.dll
 * Microsoft.CodeAnalysis.Scripting.dll
 * System.AppContext.dll
 * System.Collections.Immutable.dll
 * System.Diagnostics.StackTrace.dll
 * System.IO.FileSystem.dll
 * System.IO.FileSystem.Primitives.dll
 * System.Reflection.Metadata.dll



# 実行

## Start_NoDelete_CheckOnly.bat

実行すると、標準出力に削除対象のレジストリ一覧を出力する。削除は行わない。

## Start_Delete.bat

実行すると、削除を行なう。また、標準出力に削除対象のレジストリ一覧を出力する。


# CSScriptLauncher.exe とは

次のプロジェクトをビルドしたもの。
https://github.com/suusanex/csharp_script_launcher

