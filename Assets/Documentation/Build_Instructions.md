# Maze Outbreak - Build Instructions

## Gerar cenas e assets
1. Abra esta pasta como projeto no Unity 6000.4.2f1 ou versao compativel.
2. No menu superior, clique em `Maze Outbreak > Build Complete Project`.
3. O Unity criara materiais, prefabs, cenas e Build Settings automaticamente.

## Rodar no editor
1. Abra `Assets/Scenes/MainMenu.unity`.
2. Pressione Play.

## Gerar executavel independente para PC
Opcao automatica para Windows:
1. Instale o modulo `Windows Build Support` pelo Unity Hub, se ainda nao estiver instalado.
2. No Unity, clique em `Maze Outbreak > Build Windows 64-bit`.
3. O executavel sera criado em `Builds/MazeOutbreak_Windows/MazeOutbreak.exe`.

Opcao automatica para macOS:
1. No Unity, clique em `Maze Outbreak > Build macOS Standalone`.
2. O app sera criado em `Builds/MazeOutbreak_macOS/Maze Outbreak.app`.

Opcao manual:
1. Abra `File > Build Settings`.
2. Selecione `Windows, Mac, Linux`.
3. Escolha o alvo desejado:
   - Windows: `Standalone Windows 64-bit`.
   - macOS: `Standalone macOS`.
   - Linux: `Standalone Linux 64-bit`.
4. Clique em `Build`.
5. Escolha a pasta `Builds`.

## Build gerado nesta maquina
- macOS standalone: `Builds/MazeOutbreak_macOS/Maze Outbreak.app`.
- O modulo `WindowsStandaloneSupport` nao esta instalado neste Unity local. Para gerar `Maze Outbreak.exe`, instale o modulo Windows Build Support pelo Unity Hub e use `Standalone Windows 64-bit`.

## Build automatico macOS
No menu superior do Unity, use `Maze Outbreak > Build macOS Standalone` para regenerar cenas e criar novamente o app em `Builds/MazeOutbreak_macOS`.

## Observacao
O projeto usa scripts C#, placeholders nativos da Unity para o labirinto e assets externos importados para o zumbi, texturas do labirinto e trofeu de saida.
