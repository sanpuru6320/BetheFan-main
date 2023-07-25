# FolderBのコピーをFolderCに作る
copy-item C:\Tikemon RPG C:\bat -Recurse
# robocopyで差異がないファイルだけFloderCからFolderAに移動する
robocopy C:\bat E:\Tikumon RPG /xc /xn /xo /xx /xl /is /E /move