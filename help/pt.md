# Ajuda do ChangeTracker

Guia offline da prévia. O aplicativo observa configurações sem alterá-las. Não repara o Windows nem decide se o computador é seguro.

## Idioma

Escolha **Configurações > Idioma**. A escolha fica salva e atualiza a interface, a ajuda aberta, datas e relatórios de texto sem reiniciar ou coletar. As vinte traduções estão no aplicativo, sem internet. Árabe, árabe egípcio e urdu usam conteúdo da direita para a esquerda; o menu continua à esquerda.

Nomes de programas, nomes que você digitou, caminhos, identificadores e valores coletados não são traduzidos. JSON/CSV mantêm campos estáveis em inglês. Diálogos do Windows e UAC seguem o idioma do sistema. Ainda é necessária revisão por falantes nativos.

## Configurações

Abra Configurações no menu. As preferências são salvas para a pasta de histórico atual e restauradas ao reabrir.

- Aparência oferece tema claro/escuro, fonte e cores independentes de texto do app, rótulos, fundo e texto dos botões. Amostras nomeadas oferecem Padrão, Azul-marinho, Verde-floresta, Bordô e Roxo. Padrão restaura a cor do tema; o alto contraste do Windows tem prioridade e ações principais mantêm texto contrastante.
- Capturas automáticas vêm desativadas. Intervalos: 15 minutos, 1 hora, 6 horas, diário ou semanal. Só funcionam com o app aberto, inclusive na bandeja, sem elevação e com cancelamento. Não despertam o PC. O vencimento é verificado a cada minuto; ao reabrir pode ocorrer uma verificação atrasada, sem repetir cada intervalo perdido.
- A retenção padrão é para sempre. Com 30, 90, 180 ou 365 dias, só capturas antigas sem nome que não sejam referências são excluídas. A limpeza ocorre no primeiro vencimento, depois diariamente enquanto o app está aberto e após verificações automáticas bem-sucedidas; funciona também com capturas automáticas desativadas. Pontos nomeados e referências de todos os escopos são protegidos.
- Iniciar ao entrar no Windows é opcional e desativado por padrão. Inclui entrar depois de reiniciar, não coleta antes do login. Só a entrada de início do app para este usuário muda; não instala serviço ou tarefa de inicialização nem altera outros apps ou políticas. Uma falha preserva a escolha anterior.
- Continuar na bandeja é opcional e desativado. Minimizar ou Fechar oculta a janela enquanto as verificações continuam. Abrir ou iniciar outra cópia restaura a janela. Sair pela bandeja cancela a coleta e encerra. Sem essa opção, Fechar cancela e encerra.

Perfis novos começam com ambos os escopos marcados; escolhas salvas são preservadas. Todo o histórico retido pode ser consultado em qualquer escopo atual, mas os extremos da comparação devem ter escopo e acesso iguais. Nenhuma preferência concede administrador. Perfilamento de recursos e validação de pacotes instalados continuam pendentes.

## Primeiros passos

1. Abra normalmente, não como administrador.
2. Confira **Usuário atual** e **Todo o computador**, marcados por padrão num perfil novo. Mantenha um ou ambos, nunca nenhum, e confirme.
3. Revise **Fontes**. Rede e PATH são opcionais. Selecionar não inicia a coleta.
4. Escolha **Hoje (nova captura)** e **Verificar agora**.

A primeira observação utilizável cria a referência do escopo e acesso. É inventário, não reconstrução do passado. Perfis novos não coletam automaticamente; intervalos ativados só funcionam enquanto o app está aberto.

## Simples e Avançado

Simples apresenta resumo, comparações e texto. Avançado mostra todos os campos coletados, valores antes/depois sem limite do resumo, metadados e JSON/CSV. Datas, histórico e fontes existem nos dois modos.

A troca não coleta, eleva, move a referência ou cobra algo. Preço previsto: US$0,99 uma vez, incluindo ambos os modos. A prévia não tem compra.

## Escopos independentes

Usuário atual inclui registros próprios de apps, Run/RunOnce, padrões, áudio, proxy e PATH. Computador inclui registros compartilhados, serviços, tarefas, atualizações, drivers, firewall, DNS/DHCP e PATH do sistema. Não carrega perfis privados alheios.

As duas partes são lidas separadamente e reunidas numa captura combinada. Apenas a parte do computador pode receber administrador, mediante pedido explícito. A parte do usuário conserva a conta comum original. Escolhas anteriores e fontes por escopo são lembradas.

## Datas e capturas

Os seletores oferecem apenas capturas retidas com data local, hora com milissegundos, deslocamento UTC, ponto e escopo/acesso. Não aceitam datas digitadas livremente; capturas excluídas somem da lista. O segundo extremo pode ser uma captura salva ou uma nova verificação de Hoje. Referência seleciona a referência normal sem substituí-la. Somente estado atual limpa a escolha anterior.

Um dia sem captura não pode ser reconstruído. Não há substituição silenciosa pela data mais próxima. As observações precisam ser distintas, cronológicas, não sobrepostas e compatíveis em escopo e acesso.

## Duas capturas salvas

Escolha data e captura anteriores, depois Captura salva e data/captura posteriores. Comparar capturas não executa coletor, não pede UAC e não cria registro. A referência permanece. Duas horas do mesmo dia podem ser comparadas.

## Captura versus hoje

Escolha uma captura anterior e Hoje. Verificar agora produz uma observação nova e compara com sua seleção, não com outra referência escolhida escondida. Após sucesso o painel recolhe; pode reabri-lo.

Referência com administrador não eleva automaticamente. Use a ação explícita ou somente estado atual. Cancelar interrompe a coleta e preserva o histórico. Com a bandeja ativada, Fechar mantém a coleta; Sair pela bandeja cancela e encerra o app.

## Administrador

Muitas configurações do computador são legíveis normalmente. As protegidas ficam como lacunas. A ação de administrador exige clique, confirmação padrão Não e autorização UAC para uma única verificação.

A janela fica sem elevação. Um auxiliar temporário somente leitura verifica o computador, sem serviço ou permissão permanente. Negar não muda histórico nem referência. Não compartilhe senhas ou desative políticas. Uma solicitação UAC já visível deve ser respondida no Windows, não pode ser fechada pela aplicação.

## Entender os detalhes

Adicionado, Removido e Alterado descrevem diferenças entre observações, não autor, momento exato ou causa. Importante/Revisar são prioridades, não vereditos de malware. Atividade rotineira/esperada e impacto não avaliado ficam separados. O título conta o filtro atual; Ver todos revela além dos três primeiros.

Avançado mostra campos adicionados/removidos, contexto inalterado, valores longos, identidade e origem. Metadados incluem IDs, escopo/acesso, tempos UTC de captura/leitura, versões, estado e contagens. Vazio e ausente são distintos. Comandos nunca salvos não são recuperáveis; chaves e impressões digitais seguem escondidas. Identificadores podem identificar você: revise antes de copiar.

Marcar esperado afeta apenas a ocorrência e é reversível. Abrir configurações abre uma ferramenta permitida do Windows, sem reparar nada.

## Cobertura

Sucesso é completo no subconjunto implementado, não em todo o Windows. Parcial indica entradas faltantes ou limite; Falha, leitura inútil; Desativado/fora de escopo, não lido. Dados incompletos nunca viram exclusões presumidas.

Uma leitura atual completa ainda pode não comparar com uma antiga incompleta ou formato/chave incompatível. Uma parte incompleta torna parcial a categoria combinada. Detalhes de cobertura mantém a referência e áreas sem mudanças. Ausência de diferença não garante segurança ou causa.

## Referência e histórico

Capturas permite ver, nomear (1–120 caracteres), excluir e alterar a referência após confirmação. Troque uma referência antes de excluí-la. Usuário, computador, ambos, níveis de acesso e escopo misto antigo têm referências separadas.

Não há limite fixo de pontos. A retenção opcional limpa capturas antigas sem nome e protege pontos nomeados e todas as referências. Uma captura totalmente inútil não é salva. Limpar histórico exclui capturas e marcas após confirmação, preserva preferências e chave, e não altera exports ou Windows. Não é apagamento forense.

## Fontes e limites

| Fonte | Limitação |
| --- | --- |
| Apps e início | Registros de desinstalação e Run/RunOnce; não Store, portáteis ou pasta Inicializar. Registro não prova execução. |
| Serviços e tarefas | Configuração acessível; não execução, comandos/XML salvos ou consultas contínuas. |
| Atualizações e drivers | Histórico local bem-sucedido, limite 5.000 eventos (acima disso parcial), metadados WMI; sem instalar, firmware ou reversão. |
| Padrões e áudio | Associações suportadas e dispositivos padrão; sem gravação ou modificação. |
| Proteção | Perfis do firewall, não antivírus. |
| Rede e PATH | Opcionais: proxy ou DNS/DHCP e PATH persistente; sem pacotes, senhas, sondas ou outras variáveis. |

Cada fonte tem 25 segundos. A tela mostra 1.000 registros por fonte; todos os capturados permanecem salvos. Somente leitura permite histórico próprio e exports pedidos, não mudanças nas configurações monitoradas.

## Relatórios

Relatório usa o resultado mostrado, não datas ainda não executadas. Prévia, cópia e texto nos dois modos; JSON/CSV em Avançado. Texto traduzido, esquema estruturado estável. Nada é enviado automaticamente.

Chaves, impressões digitais, comandos, nomes de pontos e IDs de áudio são omitidos. Caminhos de perfil e padrões de credenciais são mascarados; CSV neutraliza fórmulas. Nomes identificadores podem permanecer. PDF/HTML, importação e pacotes criptografados não estão implementados. Exports continuam após limpar histórico.

## Privacidade e armazenamento

Pasta usual: `%LOCALAPPDATA%\PCChangeTracker`; Configurações mostra a real. SQLite não é criptografada. A chave usa DPAPI do usuário atual; copiar para outra conta não garante descriptografia. Faça backup seguro antes de novas versões.

Formato de histórico 2 preserva registros antigos como mistos e impede leitores antigos. O auxiliar recebe categorias e chave temporária, nunca caminho de histórico ou comandos arbitrários. Só a interface comum grava. O ciclo dos dados MSIX precisa de testes separados.

## Acessibilidade

Tab/Shift+Tab, setas e espaço navegam. Escopos têm caixas independentes, modos têm rádio. Tipo e prioridade são texto, não só cor. Há foco visível e cores de alto contraste do Windows.

F1 abre ajuda, Ctrl+F busca, Escape fecha. Zoom até 160%; tabelas estreitas viram entradas rotuladas. Qualificação completa de leitores de tela e revisão nativa permanecem pendentes.

Os títulos expõem níveis para navegação por leitor de tela. Abrir detalhes move o foco para o painel; Tab percorre seus controles e Escape fecha. Fonte e cores adaptadas ao tema ficam em Configurações; alto contraste tem prioridade.

## Problemas comuns

Data vazia: escolha outra observação. Comparação recusada: confira ordem, escopo e acesso. Parcial não significa removido. Relatório antigo: execute a nova seleção. Banco inacessível: confira espaço, permissões e versão antes de excluir.

Para suporte compartilhe relatório revisado e versões da app/Windows, nunca senha, banco bruto ou chave. Negar administrador não impede a verificação normal.

## Publicação

Prévia com 11 categorias limitadas, verificações manuais ou agendadas opcionais e retenção configurável. Não há monitor contínuo de eventos, notificações ou linha temporal completa. Coleta consome recursos; não há promessa de CPU zero.

MSI/MSIX x64 locais sem assinatura. Instalar MSI pede aprovação separada da coleta; não instale os dois formatos juntos. UAC real, outra conta administradora, Windows 10/ARM64, instalação e aprovação Store de `allowElevation` seguem pendentes. Alterar fonte não recompila instaladores antigos. Logos não substituem capturas reais nem certificação.