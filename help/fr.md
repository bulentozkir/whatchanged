# Aide ChangeTracker

Guide hors ligne de la préversion. L'application observe la configuration sans la modifier. Elle ne répare pas Windows et ne garantit pas la sécurité du PC.

## Langue

Choisissez **Paramètres > Langue**. Le choix mémorisé actualise interface, aide ouverte, dates et texte des rapports sans redémarrage ni collecte. Vingt langues sont intégrées. Arabe, arabe égyptien et ourdou se lisent de droite à gauche ; le menu reste à gauche.

Noms d'applications, libellés saisis, chemins, identifiants et valeurs originales ne sont pas traduits. JSON/CSV gardent un schéma anglais stable. Les dialogues Windows/UAC utilisent la langue système. Une révision linguistique native reste nécessaire.

## Paramètres

Ouvrez Paramètres dans le menu. Les préférences sont enregistrées pour le dossier d'historique actuel et restaurées au redémarrage.

- Apparence propose thème clair/sombre, police et couleurs indépendantes du texte, des libellés, du fond et du texte des boutons. Les échantillons nommés proposent Par défaut, Bleu marine, Vert forêt, Bordeaux et Violet. Par défaut rétablit la couleur du thème ; le contraste élevé de Windows prévaut et les actions principales conservent un texte contrastant.
- Les captures automatiques ont lieu toutes les 4 heures par défaut ; les choix enregistrés, dont Désactivé, sont conservés. Intervalles : 15 minutes, 1 heure, 4 heures, 6 heures, chaque jour ou semaine. Choisissez Désactivé pour des vérifications manuelles uniquement. Elles fonctionnent seulement lorsque l'app est ouverte, même dans la zone de notification, sans élévation et avec annulation possible. Après confirmation de la portée, la première vérification ou une échéance dépassée peut démarrer au prochain contrôle par minute ; les suivantes respectent l'intervalle. Elles ne réveillent pas le PC et ne rejouent pas les intervalles manqués.
- La conservation est de 30 jours par défaut ; les choix enregistrés, dont la conservation illimitée, sont préservés. Choisissez 30, 90, 180 ou 365 jours, ou une durée illimitée. Seuls les anciens relevés sans nom qui ne sont pas des références sont supprimés. Le nettoyage a lieu à la première échéance, puis quotidiennement pendant l'exécution et après les captures automatiques réussies, même si la fréquence automatique est désactivée. Points nommés et références de toutes les portées sont protégés.
- Le démarrage à la connexion est facultatif et désactivé par défaut. Il inclut la connexion après redémarrage, pas une collecte avant connexion. Seule l'entrée de démarrage de l'app pour cet utilisateur change ; aucun service ou tâche au démarrage, aucune autre app ou politique ne sont modifiés. En cas d'échec, le choix précédent est conservé.
- Réduire ou Fermer masque toujours la fenêtre dans la zone de notification et laisse les vérifications continuer. Ouvrir, un double-clic sur l'icône ou un nouveau lancement restaure la fenêtre. Quitter dans cette zone annule le travail et termine l'app. Le lancement normal ouvre une fenêtre agrandie ; la restauration conserve son dernier état visible. Le démarrage facultatif à la connexion reste masqué, même après un redémarrage de Windows ; aucun fonctionnement avant connexion ni service.

Simple affiche les champs publics modifiés avec des libellés Avant/Après et des valeurs plus grandes, sélectionnables et en lecture seule. Fermer les détails rend le focus au bouton d'origine s'il est encore disponible. Avancé sépare les dates conservées des heures exactes, avec point de contrôle et portée/accès sur des lignes distinctes. Changer de mode conserve la comparaison sélectionnée.

Les deux portées sont cochées sur un nouveau profil ; les choix sauvegardés sont respectés. Tous les relevés conservés sont consultables indépendamment de la portée courante, mais les deux extrémités doivent avoir les mêmes portée et accès. Aucun réglage ne donne des droits administrateur. Le profilage des ressources et la qualification des paquets installés restent à faire.

## Bien démarrer

1. Ouvrez normalement, sans droits administrateur.
2. Vérifiez les cases **Utilisateur actuel** et **Tout l'ordinateur**, cochées par défaut pour un nouveau profil. Conservez une ou deux cases, jamais zéro, puis confirmez.
3. Consultez **Sources**. Toutes les vérifications compatibles, dont Réseau et PATH, sont activées par défaut. Les choix enregistrés sont conservés ; vous pouvez désactiver des sources. Sélectionner ne lance rien.
4. Choisissez **Aujourd'hui (nouveau relevé)** puis **Vérifier maintenant**.

Le premier relevé utilisable crée une référence propre à sa portée et à son accès. C'est un inventaire, pas une reconstruction du passé. Les nouveaux profils utilisent un intervalle de 4 heures, uniquement après confirmation de la portée et pendant l'exécution. Choisissez Désactivé pour des vérifications manuelles uniquement.

## Simple et Avancé

Simple propose résumés et rapports texte. Avancé ajoute **Tous les champs enregistrés**, avant/après sans limite de résumé, métadonnées et JSON/CSV. Comparaisons par date, historique et sources sont disponibles dans les deux modes.

Changer de mode ne collecte pas, n'élève pas les droits, ne déplace pas la référence et ne coûte rien de plus. Prix prévu : 0,99 USD en achat unique. Pas de paiement dans cette préversion.

## Portées indépendantes

Utilisateur actuel lit ses applications enregistrées, Run/RunOnce, associations, audio, proxy et PATH. L'ordinateur lit registres partagés, services, tâches, mises à jour, pilotes, pare-feu, DNS/DHCP et PATH système. Aucun profil privé d'autrui n'est chargé.

Avec les deux cases, les parties sont lues séparément puis combinées. Seule la partie machine peut être élevée sur demande ; l'utilisateur reste le compte normal d'origine. Choix existants et sources par portée sont mémorisés.

## Dates et observations

Les sélecteurs proposent uniquement les relevés conservés, avec date locale, heure avec millisecondes, décalage UTC, point et portée/accès. Aucune date libre ne peut être saisie ; les relevés supprimés disparaissent. La seconde extrémité peut être un relevé enregistré ou une nouvelle vérification Aujourd'hui. **Référence** choisit la référence normale sans la remplacer. **État actuel seulement** efface la sélection antérieure.

Un jour sans relevé ne peut être reconstruit. Aucune substitution automatique par une date voisine. Les instantanés doivent être différents, chronologiques, sans chevauchement, de mêmes portée et accès.

## Deux relevés enregistrés

Choisissez date et instantané antérieurs, puis **Instantané enregistré**, date et instantané postérieurs. **Comparer les instantanés** ne lance aucun collecteur, ne demande pas UAC et ne crée rien. La référence ne change pas. Deux heures d'un même jour peuvent se comparer.

## Un relevé et aujourd'hui

Choisissez le relevé voulu et **Aujourd'hui (nouveau relevé)**. La vérification capture l'état neuf et le compare à cette sélection, pas à une référence remplacée en secret. Le panneau se replie après réussite ; rouvrez-le si besoin.

Une référence administrateur ne provoque jamais d'élévation automatique. Utilisez l'action explicite ou l'état actuel seulement. Annuler arrête la capture et conserve l'historique. Fermer laisse la collecte continuer dans la zone de notification ; Quitter l'annule et termine l'app.

## Accès administrateur

De nombreux paramètres machine sont lisibles normalement. Les autres restent des lacunes. **Vérifier en administrateur** exige votre clic, une confirmation par défaut sur Non, puis l'accord UAC pour une seule vérification.

La fenêtre principale reste non élevée. Un auxiliaire temporaire en lecture seule traite la machine, sans service ni droit permanent. Refuser ne change ni référence ni historique. Ne partagez pas de mots de passe et ne contournez pas les politiques. L'app ne peut pas fermer une invite UAC déjà affichée : répondez dans Windows.

## Comprendre les changements

Ajouté, Supprimé et Modifié décrivent les extrémités observées, pas un auteur, une cause ou un instant exact. Important/À vérifier sont des priorités, pas un verdict de malware. Activité habituelle/attendue et impact non évalué restent séparés. Le titre compte le filtre actif ; Voir tout expose les résultats au-delà des trois premiers.

Avancé montre valeurs longues, contexte inchangé, champs ajoutés/supprimés et identités/source. Les métadonnées comprennent IDs, portée/accès, dates UTC de capture/lecture, versions, états et nombres. Vide et absent sont distincts. Les commandes jamais conservées ne peuvent être restaurées ; clés et empreintes restent cachées. Les identifiants peuvent révéler l'appareil : vérifiez avant copie.

Le marquage attendu est réversible et limité à cette occurrence. Ouvrir les paramètres ouvre un outil Windows autorisé, sans réparation.

## Couverture et incertitude

Réussi signifie complet dans le sous-ensemble implémenté, pas tout Windows. Partiel signale une limite ou des entrées manquantes ; Échec, lecture inutilisable ; Désactivé/hors portée, non lu. Une lecture incomplète ne produit pas de suppressions supposées.

Un relevé actuel complet peut être incomparable à un ancien incomplet ou incompatible en format/clé. Une partie incomplète rend la catégorie combinée partielle. Détails de couverture contient référence et zones inchangées. Aucune conclusion générale de sécurité ou de causalité.

## Références et historique

Instantanés permet consulter, nommer (1–120 caractères), supprimer et changer la référence après confirmation. Remplacez une référence avant suppression. Utilisateur, machine, les deux, niveaux d'accès et anciens relevés mixtes ont des références distinctes.

Aucune limite fixe de points. La conservation facultative nettoie les anciens relevés sans nom et protège les points nommés et toutes les références. Un résultat entièrement inutilisable n'est pas sauvegardé. Effacer l'historique supprime relevés et annotations après confirmation, conserve préférences et clé, et ne touche pas aux exports ou à Windows. Ce n'est pas un effacement forensique.

## Sources et limites

| Source | Limite |
| --- | --- |
| Apps et démarrage | Registres de désinstallation et Run/RunOnce, pas Store, apps portables ou dossiers de démarrage. L'inscription ne prouve pas l'exécution. |
| Services et tâches | Configuration accessible, aucune exécution, commandes/XML stockés ou surveillance périodique. |
| Mises à jour et pilotes | Historique local réussi limité à 5 000 événements (sinon partiel), métadonnées WMI ; aucune installation, firmware ou restauration. |
| Associations et audio | Formats pris en charge et périphériques par défaut ; aucune capture sonore ou modification. |
| Protection | Profils pare-feu uniquement, pas d'antivirus. |
| Réseau et PATH | Activés par défaut : proxy ou DNS/DHCP et PATH persistant. Aucun mot de passe, paquet, sonde ou autre variable. |

Toutes les sources compatibles sont activées par défaut sans choix valide enregistré. Une source désactivée reste désactivée après une mise à jour ; modifiez-la dans Sources. L'activer ne lance aucune collecte et n'élève pas les droits. Il faut des observations utilisables aux deux extrémités pour montrer des différences ; les anciens relevés ne sont pas complétés rétroactivement.

25 secondes maximum par source. L'inventaire affiche 1 000 lignes par source mais conserve les données réellement recueillies. Lecture seule autorise l'historique de l'app et les exports demandés, pas la modification des paramètres surveillés.

## Rapports

Rapport décrit le résultat affiché, pas des dates choisies mais non exécutées. Prévisualisez et copiez/enregistrez du texte dans les deux modes ; Avancé ajoute JSON/CSV. Texte localisé, schéma structuré invariant. Aucun envoi automatique.

Clés, empreintes, commandes, noms de points et IDs audio sont omis des rapports. Chemins de profil et motifs de secrets sont masqués ; les formules CSV sont neutralisées. Des noms identifiants peuvent rester. PDF/HTML, import et paquets chiffrés ne sont pas implémentés. Les exports survivent à l'effacement de l'historique.

## Confidentialité et stockage

Chemin habituel : `%LOCALAPPDATA%\PCChangeTracker` ; Paramètres montre le chemin réel. SQLite n'est pas chiffrée. La clé utilise DPAPI de l'utilisateur actuel ; la copier ailleurs ne garantit pas son déchiffrement. Sauvegardez sûrement avant les versions nouvelles.

### Gérer l'espace disque

1. Consultez l'espace des relevés au-dessus d'Aide dans la barre gauche. Il reste visible sur chaque page et inclut toutes les portées de l'historique actuel.
2. Survolez la taille pour afficher les conseils. Tab permet aussi de cibler l'étiquette ; son nom et son aide sont accessibles aux lecteurs d'écran.
3. Réglez la fréquence des vérifications automatiques dans Paramètres. Un intervalle plus long produit moins de relevés futurs. Désactivé arrête les captures automatiques, sans supprimer l'historique ni arrêter le nettoyage par conservation.
4. Une conservation plus courte supprime les anciens relevés admissibles au prochain nettoyage prévu, pas immédiatement au choix. Celui-ci a lieu à échéance, puis chaque jour pendant l'exécution et après les captures automatiques réussies. Toutes les références et tous les points nommés restent protégés ; ce n'est pas une limite stricte d'espace.

La taille additionne `history.db`, `history.db-wal` et `history.db-shm` lorsqu'ils existent. Elle inclut les préférences, données auxiliaires et espaces réutilisables, pas seulement les relevés ; ce n'est pas l'allocation par blocs affichée par Windows. Exports, installation et fichier de clé sont exclus. B, KiB, MiB, GiB et TiB utilisent des multiples de 1 024 et le format numérique local.

La valeur s'actualise avec l'historique après capture, suppression ou nettoyage ; ce n'est pas une mesure continue. Taille indisponible ne signifie pas zéro. Une suppression peut laisser de l'espace réutilisable sans réduire le fichier ; même un historique vide occupe de l'espace. Aucune compactation automatique. Ne supprimez pas la base ni ses fichiers temporaires pendant l'exécution.

Le format historique 2 conserve les anciens relevés comme mixtes et refuse les anciens lecteurs. L'auxiliaire reçoit catégories et copie temporaire de clé, jamais chemin d'historique ou commande libre ; seule l'interface normale enregistre. La gestion des données MSIX nécessite des essais distincts.

## Accessibilité

Dans Paramètres > Apparence, Taille du texte propose 100%, 125%, 150% et 200%, mémorise le choix et agrandit pages, contrôles, Aide et Rapport. Les champs jumelés passent sur une colonne si nécessaire ; pages, barre latérale et dialogues défilent. La police choisie s'applique aussi au guide ; le zoom propre à l'Aide reste disponible jusqu'à 160% de cette base.

La navigation latérale annonce la page sélectionnée et utilise les flèches. Ctrl+1 ouvre les changements, Ctrl+2 les instantanés, Ctrl+3 les sources et Ctrl+4 les paramètres. F6 et Shift+F6 parcourent navigation, barre de commandes et titre de page ; Tab continue dans les contrôles. Dans l'Aide, F6 parcourt recherche, sujets et document ; Ctrl+F revient à la recherche.

Le choix de portée cible sa première case et revient à Changer la portée après confirmation. Les panneaux modaux désactivent toute la barre latérale et les raccourcis de page ; Tab reste à l'intérieur. Escape ferme les détails et rend le focus à l'action d'origine si disponible. L'Aide commence dans la recherche et le Rapport dans l'aperçu en lecture seule. Lignes de relevés, sujets, groupes et champs ont des noms lisibles ; les valeurs identifient champ et côté Avant/Après, les sources exposent état et portée. Les contrôles principaux ont une hauteur d'interaction minimale de 44 unités indépendantes du périphérique.

Il ne s'agit pas d'une certification universelle. Des essais manuels avec lecteurs d'écran, thèmes de contraste, échelles Windows et personnes en situation de handicap restent nécessaires. Les tests de clavier réel exigent une session déverrouillée et non perturbée.

Tab/Maj+Tab, flèches et espace servent à naviguer. Cases indépendantes pour les portées, radios pour les modes. Type et priorité ont un texte, pas seulement une couleur. Focus visible et couleurs Windows de contraste élevé.

F1 ouvre l'aide, Ctrl+F cherche, Échap ferme. Zoom jusqu'à 160 % ; tableaux étroits transformés en entrées étiquetées. La validation complète des lecteurs d'écran et la révision native restent à faire.

Les titres exposent des niveaux pour la navigation par lecteur d'écran. Les détails reçoivent le focus à l'ouverture ; Tab reste dans le panneau et Échap le ferme. Police et couleurs adaptatives se choisissent dans Paramètres ; le contraste élevé prévaut.

## Dépannage

Date vide : choisissez un autre relevé. Comparaison refusée : vérifiez chronologie, portée et droits. Source partielle : ce n'est pas une suppression. Rapport ancien : exécutez le nouveau choix. Base inaccessible : vérifiez espace, droits et version avant de supprimer.

Pour l'assistance, transmettez un rapport relu et les versions app/Windows, jamais mot de passe, base brute ou clé. Refuser les droits administrateur n'empêche pas les vérifications normales.

## Publication

Préversion avec 11 catégories partielles, vérifications manuelles ou planifiées facultatives et conservation configurable. Pas de surveillance continue des événements, de notifications ou de chronologie complète. La collecte consomme des ressources ; aucune promesse de CPU nul.

MSI/MSIX x64 locaux non signés ; l'installation MSI demande une autorisation séparée de la collecte. N'installez pas les deux formats. UAC réel, compte administrateur distinct, Windows 10/ARM64, installation et approbation Store `allowElevation` restent à qualifier. Modifier le code ne reconstruit pas les anciens paquets. Les logos ne remplacent ni vraies captures ni certification.