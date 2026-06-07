# Maze Outbreak - Game Design Document

## Nome do jogo
Maze Outbreak

## Conceito
Maze Outbreak e um jogo 3D em primeira pessoa de terror e sobrevivencia. O jogador acorda em uma sequencia de labirintos infectados e precisa encontrar a saida antes que zumbis patrulhando os corredores o alcancem.

## Genero
Terror, sobrevivencia, labirinto, primeira pessoa.

## Objetivo
Encontrar a saida de cada labirinto e sobreviver ate concluir a Fase 4.

## Controles
- WASD: mover.
- Mouse: olhar ao redor.
- Shift: correr.
- F: ligar ou desligar a lanterna.
- Esc: pausar ou continuar.

## Fases
- Fase 1 - Labirinto Abandonado: labirinto pequeno, pouca escuridao e 2 zumbis lentos.
- Fase 2 - Labirinto Subterraneo: labirinto maior, ambiente mais escuro e 3 zumbis.
- Fase 3 - Labirinto Contaminado: corredores estreitos, marcas visuais de contaminacao e 4 zumbis.
- Fase 4 - Labirinto Final: labirinto mais dificil, iluminacao baixa e 6 zumbis.

## Inimigos
Os zumbis usam capsulas simples como placeholder. Eles patrulham pontos do labirinto, detectam o jogador por distancia, perseguem quando o jogador entra na area de deteccao e causam Game Over ao encostar no jogador.

## Regras
- O jogador inicia no menu principal.
- Iniciar Jogo carrega a Fase 1.
- Selecionar Fase permite abrir qualquer uma das 4 fases.
- Ao chegar na saida, o jogador avanca para a proxima fase.
- Ao ser tocado por um zumbi, a tela de Game Over aparece imediatamente.
- A tecla Esc abre a pausa durante as fases.

## Condicao de vitoria
Encontrar a saida final da Fase 4.

## Condicao de derrota
Ser encostado por qualquer zumbi em qualquer fase.

## Assets utilizados
- Labirintos placeholder gerados com cubos para paredes e cubos achatados para o piso.
- Texturas Backrooms aplicadas ao piso, paredes e teto do labirinto placeholder.
- Materiais simples para diferenciar as fases e areas contaminadas.
- Capsula para jogador.
- Modelo 3D `ZOMBIEN_3D_MODEL_FREE_BY_Oscar_Creativo.fbx` para os zumbis.
- Texturas de pele, olhos, dentes, lingua e normais aplicadas ao material do zumbi.
- Modelo 3D `Trofeu_Libertadores.fbx` como objeto de saida/coleta para avancar de fase.
- Musica ambiente `intense_horror_music_01.mp3` tocando em volume baixo durante as fases.
- Som `passos-terror.mp3` usado como ambiencia baixa de passos de zumbis durante as fases.
- Materiais simples para portas, efeitos e luzes fluorescentes.
- Luz direcional fraca, paineis fluorescentes e lanterna do jogador.
- Canvas nativo da Unity para menus, HUD, pausa, Game Over e Vitoria.

## Storyboard textual das fases
1. O jogador entra no Labirinto Abandonado, com corredores simples e luz ambiente moderada. A prioridade e aprender a movimentacao e escapar dos primeiros zumbis lentos.
2. O Labirinto Subterraneo aumenta a escala e reduz a luz. O jogador precisa usar a lanterna com mais frequencia e escolher rotas com cuidado.
3. O Labirinto Contaminado apresenta corredores mais apertados e manchas verdes de contaminacao, criando uma sensacao de perigo biologico.
4. O Labirinto Final tem menos iluminacao, mais zumbis e caminho mais longo. Ao encontrar a porta final, o jogador vence o jogo.
