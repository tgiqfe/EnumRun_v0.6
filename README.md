# EnumRun
Ver. 0.6系

## 使い方

基本情報をセット
```powershell
Set-EnumRunSetting
```

予め.ps1ファイルを作成し、ローカルGPOに設定した状態で、
```powershell
# スタートアップスクリプト
Enter-StartupScript

# ログオンスクリプト
Enter-LogonScript

# ログオフスクリプト
Enter-LogoffScript

# シャットダウンスクリプト
Enter-ShutdownScript
```
