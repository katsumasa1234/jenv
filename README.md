# jenv

jdkのバージョン管理を容易に行えるようにします。

## 動作環境

Widows11 x64

その他デバイスでは動作確認はされていません。

## 環境構築

jenv-versoin.zipをダウンロードして、展開してください。

展開したファイルの中に入っているjenvというディレクトリを好きな場所へ移動させ、パスをコピーしてください。
(例: C:\jenv)

JENV_HOMEという環境変数を新たに作成し、先ほどコピーしたパスを設定してください。

Pathという環境変数に%JAVA_HOME%\binというパスを追加してください。

これで環境構築は完了です。

## 使用方法

パスの追加が正しく行えていれば、コマンドプロンプトでjenvというコマンドが使えるようになっているはずです。

### jenv help

ヘルプを表示します。

### jenv version

現在設定されているjdkのバージョンを表示します。

### jenv versions

インストールされているjdkの一覧を表示します。

### jenv change version

指定したバージョンにjdkを変更します。

入力例：jenv change jdk-20.0.2+9

### jenv list

インストール可能なjdkのバージョンを表示します。

出力例：

8
11
16
17
18
19
20
21
22

### jenv list version

指定したバージョンでのインストール可能なjdkの一覧を表示します。

入力例：jenv list 8

出力例：

jdk8u422-b05
jdk8u412-b08
jdk8u402-b06
jdk8u392-b08
jdk8u382-b05
jdk8u372-b07
jdk8u362-b09
jdk8u352-b08
jdk8u345-b01
jdk8u342-b07.1
jdk8u342-b07
jdk8u332-b09
jdk8u322-b06
jdk8u312-b07
jdk8u302-b08.1
jdk8u302-b08

### jenv install version

指定したバージョンのjdkをインストールします。

入力例：jenv install jdk8u422-b05

### jenv delete version

指定したバージョンのjdkを削除します。

入力例：jenv delete jdk8u422-b05

## 注意点

jdkはすべて%JENV_HOME%\javaにインストールされます。

jdkの切り替えはJAVA_HOMEを書き換えることによって実現しています。

このプログラムの使用は自己責任でお願いします。

このREADMEの最終更新はjenv-1.0のリリース時です。
