# Ajuda do ChangeTracker

Guia offline da prévia. O aplicativo observa configurações sem alterá-las. Não repara o Windows nem decide se o computador é seguro.

## Idioma

Escolha **Configurações > Idioma**. A escolha fica salva e atualiza a interface, a ajuda aberta, datas e relatórios de texto sem reiniciar ou coletar. As vinte traduções estão no aplicativo, sem internet. Árabe, árabe egípcio e urdu usam conteúdo da direita para a esquerda; o menu continua à esquerda.

Nomes de programas, nomes que você digitou, caminhos, identificadores e valores coletados não são traduzidos. JSON/CSV mantêm campos estáveis em inglês. Diálogos do Windows e UAC seguem o idioma do sistema. Ainda é necessária revisão por falantes nativos.

## Configurações

Abra Configurações no menu. As preferências são salvas para a pasta de histórico atual e restauradas ao reabrir.

- Aparência oferece tema claro/escuro, fonte e cores independentes de texto do app, rótulos, fundo e texto dos botões. Amostras nomeadas oferecem Padrão, Azul-marinho, Verde-floresta, Bordô e Roxo. Padrão restaura a cor do tema; o alto contraste do Windows tem prioridade e ações principais mantêm texto contrastante. Sem uma escolha salva, o tema escuro é o padrão. Os botões seguem uma hierarquia clara: a verificação principal é azul, substituir a referência é âmbar, as exclusões são vermelhas e os demais comandos são neutros com um ícone colorido (por exemplo Relatório, Alterar escopo e Ajuda). As cores foram pensadas para baixa visão e daltonismo: o que é interativo é azul, os rótulos têm cor própria e cada estado também tem uma palavra e um símbolo. Listas suspensas, caixas de seleção e chaves usam a cor de destaque na seta, na marca e no contorno de foco. As seções de Configurações aparecem em colunas, então a página costuma caber em uma única tela sem rolagem.
- Capturas automáticas ocorrem a cada 4 horas por padrão; escolhas salvas, incluindo Desativado, são preservadas. Intervalos: 15 minutos, 1 hora, 4 horas, 6 horas, diário ou semanal. Escolha Desativado para verificações apenas manuais. Só funcionam com o app aberto, inclusive na bandeja, sem elevação e com cancelamento. Após confirmar o escopo, a primeira verificação ou uma atrasada pode ocorrer na próxima checagem por minuto; as seguintes respeitam o intervalo. Não despertam o PC nem repetem todos os intervalos perdidos.
- A retenção padrão é de 30 dias; escolhas salvas, incluindo para sempre, são preservadas. Escolha 30, 90, 180 ou 365 dias, ou para sempre. Só capturas antigas sem nome que não sejam referências são excluídas. A limpeza ocorre no primeiro vencimento, depois diariamente enquanto o app está aberto e após verificações automáticas bem-sucedidas; funciona também com capturas automáticas desativadas. Pontos nomeados e referências de todos os escopos são protegidos.
- Iniciar ao entrar no Windows é opcional e desativado por padrão. Inclui entrar depois de reiniciar, não coleta antes do login. Só a entrada de início do app para este usuário muda; não instala serviço ou tarefa de inicialização nem altera outros apps ou políticas. Uma falha preserva a escolha anterior.
- Minimizar ou Fechar sempre oculta a janela na bandeja enquanto as verificações continuam. Abrir, clicar duas vezes no ícone ou iniciar outra cópia restaura a janela. Sair pela bandeja cancela a coleta e encerra. O início normal abre maximizado; a restauração mantém o último estado visível. O início opcional ao entrar começa oculto, inclusive após reiniciar o Windows; não funciona antes do login nem como serviço.

Simples mostra os campos públicos alterados com rótulos Antes/Depois e valores maiores, selecionáveis e somente leitura. Fechar os detalhes devolve o foco ao botão de origem, se disponível. Avançado separa as datas retidas dos horários exatos de captura, com ponto de controle e escopo/acesso em linhas distintas. Mudar de modo preserva a comparação selecionada.

Perfis novos começam com ambos os escopos marcados; escolhas salvas são preservadas. Todo o histórico retido pode ser consultado em qualquer escopo atual, mas os extremos da comparação devem ter escopo e acesso iguais. Nenhuma preferência concede administrador. Perfilamento de recursos e validação de pacotes instalados continuam pendentes.

## Primeiros passos

1. Abra normalmente, não como administrador.
2. Confira **Usuário atual** e **Todo o computador**, marcados por padrão num perfil novo. Mantenha um ou ambos, nunca nenhum, e confirme.
3. Revise **Fontes**. Todas as verificações compatíveis, incluindo Rede e PATH, são ativadas por padrão. Escolhas salvas são preservadas; você pode desativar fontes. Selecionar não inicia a coleta.
4. Escolha **Hoje (nova captura)** e **Verificar agora**.

A primeira observação utilizável cria a referência do escopo e acesso. É inventário, não reconstrução do passado. Perfis novos usam intervalo de 4 horas, somente após confirmar o escopo e com o app aberto. Escolha Desativado para verificações apenas manuais.

## Simples e Avançado

Simples apresenta resumo, comparações e texto. Avançado mostra todos os campos coletados, valores antes/depois sem limite do resumo, metadados e JSON/CSV. Datas, histórico e fontes existem nos dois modos.

A troca não coleta, eleva, move a referência ou cobra algo. Preço previsto: US$0,99 uma vez, incluindo ambos os modos. A prévia não tem compra.

## Escopos independentes

Usuário atual inclui registros próprios de apps, Run/RunOnce, padrões, áudio, proxy e PATH. Computador inclui registros compartilhados, serviços, tarefas, atualizações, drivers, firewall, DNS/DHCP e PATH do sistema. Não carrega perfis privados alheios.

As duas partes são lidas separadamente e reunidas numa captura combinada. Os dois escopos usam acesso normal com sua própria conta. Escolhas anteriores e fontes por escopo são lembradas.

## Datas e capturas

Os seletores oferecem apenas capturas retidas com data local, hora com milissegundos, deslocamento UTC, ponto e escopo/acesso. Não aceitam datas digitadas livremente; capturas excluídas somem da lista. O segundo extremo pode ser uma captura salva ou uma nova verificação de Hoje. Referência seleciona a referência normal sem substituí-la. Somente estado atual limpa a escolha anterior. O cabeçalho **Comparação** sempre informa as duas extremidades da escolha, então continua legível quando recolhido; ele se recolhe após uma verificação ou comparação e ao abrir outra página.

Um dia sem captura não pode ser reconstruído. Não há substituição silenciosa pela data mais próxima. As observações precisam ser distintas, cronológicas, não sobrepostas e compatíveis em escopo e acesso.

## Duas capturas salvas

Escolha data e captura anteriores, depois Captura salva e data/captura posteriores. Comparar capturas não executa coletor, não pede UAC e não cria registro. A referência permanece. Duas horas do mesmo dia podem ser comparadas.

## Captura versus hoje

Escolha uma captura anterior e Hoje. Verificar agora produz uma observação nova e compara com sua seleção, não com outra referência escolhida escondida. Após sucesso o painel recolhe; pode reabri-lo.

Se a captura anterior usou acesso de administrador numa versão anterior, ela não pode ser a referência para uma nova verificação, porque as verificações sempre usam acesso normal. Escolha uma captura com acesso normal ou **Somente estado atual**. Duas capturas salvas ainda podem ser comparadas. Cancelar interrompe a coleta e preserva o histórico. Fechar mantém a coleta na bandeja; Sair pela bandeja cancela e encerra o app.

## Administrador

O ChangeTracker nunca pede acesso de administrador. Toda verificação, manual ou automática, usa suas permissões normais do Windows nos dois escopos; por isso o Windows nunca mostra um prompt UAC para uma verificação. Não há modo administrador, auxiliar elevado nem serviço em segundo plano.

Muitas configurações do computador inteiro são legíveis com permissões normais. Quando uma fonte contém algo que essas permissões não conseguem ler, o ChangeTracker relata essa fonte como incompleta em vez de elevar permissões. Fontes incompletas aparecem nos detalhes de cobertura e nunca sugerem remoções.

Iniciar o ChangeTracker com **Executar como administrador** não é compatível: o app mostra uma mensagem e fecha. Abra normalmente.

Capturas salvas com acesso de administrador por uma versão anterior permanecem no histórico. Você pode vê-las, compará-las entre si e incluí-las em relatórios, mas elas não podem ser a captura anterior para uma nova verificação, porque novas verificações sempre usam acesso normal. Escolha uma captura com acesso normal ou **Somente estado atual**.

Instalar o MSI requer aprovação de administrador do Windows; essa aprovação vale só para instalação, não para as verificações do ChangeTracker. O pacote da Microsoft Store instala sem ela. Nunca compartilhe senha de administrador.
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
| Rede e PATH | Ativadas por padrão: proxy ou DNS/DHCP e PATH persistente; sem pacotes, senhas, sondas ou outras variáveis. |

Todas as fontes compatíveis começam ativadas quando não há escolha válida salva. Uma fonte desativada continua assim após atualizar; altere-a em Fontes. Ativá-la não inicia coleta nem eleva permissões. São necessárias observações utilizáveis nas duas pontas para mostrar diferenças; capturas antigas não são preenchidas retroativamente.

Cada fonte tem 25 segundos. A tela mostra 1.000 registros por fonte; todos os capturados permanecem salvos. Somente leitura permite histórico próprio e exports pedidos, não mudanças nas configurações monitoradas.

## Relatórios

Relatório usa o resultado mostrado, não datas ainda não executadas. Prévia, cópia e texto nos dois modos; JSON/CSV em Avançado. Texto traduzido, esquema estruturado estável. Nada é enviado automaticamente.

Chaves, impressões digitais, comandos, nomes de pontos e IDs de áudio são omitidos. Caminhos de perfil e padrões de credenciais são mascarados; CSV neutraliza fórmulas. Nomes identificadores podem permanecer. PDF/HTML, importação e pacotes criptografados não estão implementados. Exports continuam após limpar histórico.

## Privacidade e armazenamento

Pasta usual: `%LOCALAPPDATA%\PCChangeTracker`; Configurações mostra a real. SQLite não é criptografada. A chave usa DPAPI do usuário atual; copiar para outra conta não garante descriptografia. Faça backup seguro antes de novas versões.

### Gerenciar o espaço

1. Veja o espaço das capturas acima de Ajuda na barra esquerda. Ele aparece em todas as páginas e inclui todos os escopos do histórico atual.
2. Passe o mouse sobre o tamanho para ver orientações. Tab também focaliza o rótulo; leitores de tela recebem seu nome e ajuda.
3. Ajuste a frequência das verificações automáticas em Configurações. Um intervalo maior cria menos capturas futuras. Desativado interrompe capturas automáticas, mas não apaga histórico nem desativa a limpeza por retenção.
4. Uma retenção menor remove capturas antigas elegíveis na próxima limpeza prevista, não imediatamente ao selecionar. Ela ocorre quando devida, depois diariamente durante a execução e após capturas automáticas bem-sucedidas. Referências e pontos nomeados são protegidos; não é um limite rígido de espaço.

A medida soma `history.db`, `history.db-wal` e `history.db-shm`, quando existem. Inclui preferências, dados auxiliares e espaço reutilizável, não apenas capturas; não representa a alocação por blocos mostrada pelo Windows. Exclui exportações, instalação e arquivo de chave. B, KiB, MiB, GiB e TiB usam múltiplos de 1.024 e formatação numérica local.

Atualiza com o histórico após captura, exclusão ou limpeza; não é monitoramento contínuo. Tamanho indisponível não significa zero. Excluir pode deixar espaço reutilizável sem reduzir o arquivo; até um histórico vazio ocupa espaço. Não há compactação automática. Não exclua o banco nem seus arquivos temporários com o app aberto.

Formato de histórico 2 preserva registros antigos como mistos e impede leitores antigos. Faça backup de dados importantes antes de usar uma compilação não lançada. Redirecionamento, redefinição e desinstalação de dados MSIX precisam de testes separados; não presuma que o ciclo deles seja igual ao da compilação MSI.

## Acessibilidade

Em Configurações > Aparência, Tamanho do texto oferece 100%, 125%, 150% e 200%, salva a escolha e amplia páginas, controles, Ajuda e Relatório. Campos em pares ficam em uma coluna quando necessário; páginas, barra lateral e diálogos podem rolar. A fonte escolhida também vale para o guia; o zoom próprio da Ajuda continua disponível até 160% dessa base.

A navegação lateral anuncia a página selecionada e usa as setas. Ctrl+1 abre alterações, Ctrl+2 capturas, Ctrl+3 fontes e Ctrl+4 configurações. F6 e Shift+F6 percorrem navegação, barra de comandos e título da página; Tab segue pelos controles. Na Ajuda, F6 percorre busca, tópicos e documento; Ctrl+F volta à busca.

O seletor de escopo focaliza a primeira caixa e volta a Alterar escopo após confirmar. Painéis modais desativam toda a barra lateral e atalhos de página; Tab fica dentro. Escape fecha detalhes e devolve o foco à ação original, se disponível. Ajuda começa na busca e Relatório na prévia somente leitura. Linhas de capturas, tópicos, grupos e campos têm nomes legíveis; valores identificam campo e lado Antes/Depois, e fontes informam status e escopo. Controles principais têm altura mínima de interação de 36 unidades independentes do dispositivo, acima do tamanho mínimo de alvo de 24 pixels da WCAG 2.2 AA; botões desativados mostram contorno tracejado.

Isso não é certificação universal. Ainda são necessários testes manuais com leitores de tela, temas de contraste, escalas Windows e pessoas com deficiência. Os testes de teclado real exigem sessão desbloqueada e sem interferência.

Tab/Shift+Tab, setas e espaço navegam. Escopos têm caixas independentes, modos têm rádio. Tipo e prioridade são texto, não só cor. Há foco visível e cores de alto contraste do Windows.

F1 abre ajuda, Ctrl+F busca, Escape fecha. Zoom até 160%; tabelas estreitas viram entradas rotuladas. Qualificação completa de leitores de tela e revisão nativa permanecem pendentes.

Os títulos expõem níveis para navegação por leitor de tela. Abrir detalhes move o foco para o painel; Tab percorre seus controles e Escape fecha. Fonte e cores adaptadas ao tema ficam em Configurações; alto contraste tem prioridade.

## Problemas comuns

Data vazia: escolha outra observação. Comparação recusada: confira ordem, escopo e acesso. Parcial não significa removido. Relatório antigo: execute a nova seleção. Banco inacessível: confira espaço, permissões e versão antes de excluir.

Para suporte compartilhe relatório revisado e versões da app/Windows, nunca senha, banco bruto ou chave. Algumas configurações do computador inteiro precisam de direitos de administrador; o ChangeTracker as relata como incompletas em vez de pedir elevação e continua comparando outras fontes.

## Publicação

Prévia com 11 categorias limitadas, verificações manuais ou agendadas opcionais e retenção configurável. Não há monitor contínuo de eventos, notificações ou linha temporal completa. Coleta consome recursos; não há promessa de CPU zero.

A versão local tem instaladores MSI x64 e ARM64 e um pacote MSIX x64, todos sem assinatura. A instalação MSI precisa de aprovação de administrador, mas o app instalado sempre roda com permissões normais. Assinatura, certificação da Store, qualificação do Windows 10/ARM64 e qualificação de instalar/atualizar/desinstalar continuam pendentes. Alterar fonte não recompila instaladores antigos. Logos não substituem capturas reais nem certificação.