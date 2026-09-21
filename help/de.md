# ChangeTracker-Hilfe

Offline-Anleitung zur Entwicklungsvorschau. Die App liest Konfiguration, ändert sie aber nicht. Sie repariert Windows nicht und gibt kein Sicherheitsurteil ab.

## Sprache

Wählen Sie **Einstellungen > Sprache**. Die Wahl wird gespeichert und aktualisiert Oberfläche, offene Hilfe, Datumsangaben und Textberichte ohne Neustart oder Erfassung. Alle zwanzig Sprachen sind offline enthalten. Arabisch, ägyptisches Arabisch und Urdu nutzen Rechts-nach-links-Inhalte; das Menü bleibt links.

App-Namen, eigene Bezeichnungen, Pfade, Kennungen und Originalwerte werden nicht übersetzt. JSON/CSV behalten stabile englische Felder. Windows/UAC-Dialoge nutzen die Systemsprache. Muttersprachliche Prüfung vor Veröffentlichung ist noch nötig.

## Einstellungen

Öffnen Sie Einstellungen im Menü. Die Auswahl wird für das aktuelle Verlaufsverzeichnis gespeichert und beim Neustart wiederhergestellt.

- Darstellung bietet Hell/Dunkel, Schriftart sowie unabhängige Farben für App-Text, Beschriftungen, Schaltflächenhintergrund und Schaltflächentext. Benannte Farbfelder bieten Standard, Marineblau, Waldgrün, Bordeaux und Violett. Standard setzt die jeweilige Designfarbe zurück. Der hohe Kontrast von Windows hat Vorrang; Hauptaktionen behalten kontrastierenden Text.
- Automatische Aufnahmen sind zunächst aus. Intervalle: 15 Minuten, 1 Stunde, 6 Stunden, täglich oder wöchentlich. Sie laufen nur bei geöffneter App, auch im Infobereich, mit normalen Rechten und Abbruchmöglichkeit. Sie fordern keine Administratorrechte und wecken den PC nicht. Fälligkeit wird minütlich geprüft; nach erneutem Öffnen kann eine überfällige Prüfung folgen, nicht jede verpasste einzeln.
- Aufbewahrung ist zunächst unbegrenzt. 30, 90, 180 oder 365 Tage löschen nur ältere unbenannte Aufnahmen, die keine Referenz sind. Bereinigung läuft bei erster Fälligkeit, danach täglich während die App läuft und nach erfolgreichen automatischen Prüfungen, auch bei ausgeschalteter Aufnahmefrequenz. Benannte Prüfpunkte und alle Referenzen bleiben geschützt.
- Start bei Anmeldung ist optional und zunächst aus. Das gilt auch nach einem Neustart, nicht vor der Anmeldung. Nur der eigene benutzerbezogene Starteintrag wird geändert; kein Dienst oder Systemstarttask, keine andere App oder Richtlinie. Ein Fehler lässt die vorige Auswahl bestehen.
- Im Infobereich weiterlaufen ist optional und zunächst aus. Minimieren oder Schließen verbirgt das Fenster, Prüfungen laufen weiter. Öffnen oder erneuter App-Start stellt es wieder her. Beenden im Infobereich bricht aktive Arbeit ab und beendet die App. Ohne diese Option bricht Schließen ab und beendet.

Neue Profile wählen beide Bereiche; gespeicherte Entscheidungen bleiben erhalten. Alle aufbewahrten Bereiche lassen sich unabhängig vom Erfassungsbereich durchsuchen, aber Vergleichsenden müssen bei Bereich und Rechten übereinstimmen. Einstellungen erhöhen keine Rechte. Ressourcenprofilierung und Prüfung installierter Pakete stehen noch aus.

## Einstieg

1. Normal starten, nicht als Administrator.
2. **Aktueller Benutzer** und **Gesamter Computer** prüfen. Bei neuen Profilen sind beide Kästchen an. Eines oder beide wählen, nie keines, und bestätigen.
3. Quellen prüfen. Netzwerk und PATH sind optional. Eine Auswahl startet keine Erfassung.
4. Heute und Jetzt prüfen wählen.

Die erste nutzbare Beobachtung wird zur Referenz für Umfang und Rechte. Sie ist Bestand, keine Rekonstruktion früherer Änderungen. Neue Profile erfassen nicht automatisch; aktivierte Intervalle gelten nur während die App läuft.

## Einfach und Erweitert

Einfach zeigt Zusammenfassungen und Textberichte. Erweitert ergänzt sämtliche erfassten Felder mit Vorher/Nachher-Werten, technische Metadaten und JSON/CSV. Datumsvergleiche, Verlauf und Quellen stehen in beiden Modi bereit.

Der Wechsel erfasst nichts, erhöht keine Rechte und ändert keine Referenz. Geplant sind einmalig 0,99 USD für beide Modi; die Vorschau enthält keinen Kaufvorgang.

## Unabhängige Bereiche

Der Benutzerbereich enthält eigene App-Registrierungen, Run/RunOnce, Zuordnungen, Audio, Proxy und PATH. Der Computerbereich enthält gemeinsame Registrierungen, Dienste, Aufgaben, Updates, Treiber, Firewallprofile, DNS/DHCP und System-PATH. Fremde private Profile werden nicht geladen.

Bei beiden Kästchen werden die Bereiche getrennt gelesen und zusammen gespeichert. Nur der Computerteil kann ausdrücklich erhöhte Rechte erhalten. Der Benutzerteil bleibt beim ursprünglichen normalen Konto. Bestehende Auswahl und Quellen je Umfang bleiben gespeichert.

## Datum und Aufnahmen

Die Auswahllisten bieten nur aufbewahrte Aufnahmen mit lokalem Datum, Uhrzeit samt Millisekunden, UTC-Abstand, Prüfpunkt und Bereich/Rechten. Freie Datumseingaben sind nicht möglich; gelöschte Aufnahmen verschwinden. Das zweite Ende kann eine gespeicherte Aufnahme oder eine neue Heute-Prüfung sein. Referenz wählt die normale gespeicherte Referenz, ohne sie zu ändern. Nur aktueller Zustand löscht die frühere Auswahl.

Ohne Aufnahme an einem Tag gibt es keine Daten. Es wird kein Nachbartag stillschweigend verwendet. Aufnahmen müssen verschieden, zeitlich geordnet, nicht überlappend und bei Umfang/Rechten kompatibel sein.

## Zwei gespeicherte Aufnahmen

Früheres Datum und Aufnahme wählen, dann Gespeicherte Aufnahme sowie späteres Datum und Aufnahme. Aufnahmen vergleichen startet keinen Sammler, fordert kein UAC an und speichert nichts Neues. Die Referenz bleibt gleich. Verschiedene Zeiten desselben Tages sind möglich.

## Aufnahme gegen heute

Frühere Aufnahme und Heute wählen. Jetzt prüfen erfasst frisch und vergleicht genau mit der Auswahl, nicht heimlich mit einer anderen Referenz. Der Auswahlbereich klappt nach Erfolg zu und lässt sich wieder öffnen.

Eine Administratorreferenz erhöht niemals automatisch Rechte. Die separate Adminaktion verwenden oder nur den aktuellen Zustand erfassen. Abbrechen stoppt die Prüfung und bewahrt den Verlauf. Bei aktiviertem Infobereich läuft die Erfassung nach Schließen weiter; Beenden im Infobereich bricht sie ab und beendet die App.

## Administratorrechte

Viele Computerdaten sind normal lesbar; geschützte Bereiche bleiben als Lücken sichtbar. Die Adminaktion verlangt einen expliziten Klick, eine Bestätigung mit Vorgabe Nein und Windows-UAC für eine einzelne Prüfung.

Das Hauptfenster bleibt normal. Ein kurzlebiger Nur-Lese-Helfer prüft den Computer, installiert keinen Dienst und behält keine dauerhaften Rechte. Ablehnen ändert weder Verlauf noch Referenz. Keine Kennwörter teilen oder Richtlinien abschalten. Eine bereits offene sichere UAC-Abfrage muss in Windows geschlossen werden.

## Änderungen lesen

Hinzugefügt, Entfernt und Geändert beschreiben beobachtete Endpunkte, nicht Urheber, genaue Zeit oder Ursache. Wichtig/Prüfen sind Prioritäten, keine Malwareurteile. Übliche/erwartete sowie ungeprüfte Änderungen bleiben in eigenen Gruppen. Die Überschrift zählt den Filter; Alle anzeigen öffnet mehr als die ersten drei.

Erweitert zeigt lange Werte, unveränderten Kontext, hinzugefügte/entfernte Felder sowie Quelle und Datensatzidentität. Metadaten enthalten IDs, Umfang/Rechte, UTC-Zeiten von Aufnahme und Quellenlesung, Status, Versionen und Anzahlen. Leer und nicht vorhanden sind verschieden. Nicht gespeicherte Befehle sind nicht rekonstruierbar, Schlüssel und Fingerabdrücke bleiben verborgen. Gerätekennungen können identifizieren: vor Kopieren prüfen.

Als erwartet markieren ist reversibel und betrifft nur dieses Ereignis. Einstellungen öffnen führt zu einem erlaubten Windows-Ziel, führt aber keine Reparatur aus.

## Abdeckung und Unsicherheit

Erfolgreich bedeutet vollständig im implementierten Teilumfang, nicht ganz Windows. Teilweise bedeutet fehlende Einträge oder Limit, Fehlgeschlagen unbrauchbare Lesung, Deaktiviert/außerhalb nicht gelesen. Aus unvollständigen Daten entstehen keine vermuteten Löschungen.

Eine aktuelle vollständige Aufnahme kann wegen älterer Lücken oder inkompatibler Formate/Schlüssel nicht vergleichbar sein. Bei kombinierten Bereichen macht ein unvollständiger Teil die ganze Kategorie teilweise. Abdeckungsdetails enthalten Referenz und unveränderte Bereiche. Kein allgemeines Sicherheits- oder Kausalitätsurteil.

## Referenzen und Verlauf

Unter Aufnahmen können Sie ansehen, benennen (1–120 Zeichen), löschen oder nach Bestätigung die Referenz ändern. Vor Löschen einer Referenz eine andere wählen. Benutzer, Computer, beide, Rechtevarianten und alte gemischte Aufnahmen besitzen getrennte Referenzen.

Kein festes Prüfpunktlimit. Die optionale Aufbewahrung bereinigt alte unbenannte Aufnahmen; benannte Prüfpunkte und alle Referenzen bleiben geschützt. Eine völlig unbrauchbare Erfassung wird nicht gespeichert. Verlauf löschen entfernt Aufnahmen und Markierungen nach Bestätigung, behält Einstellungen/Schlüssel und lässt Exporte/Windows unverändert. Keine forensische Löschung.

## Quellen und Grenzen

| Quelle | Grenze |
| --- | --- |
| Apps und Start | Deinstallationsregistrierungen und Run/RunOnce; keine Store-/portablen Apps oder Startordner. Registrierung beweist keine Ausführung. |
| Dienste und Aufgaben | Lesbare Konfiguration, keine Ausführung, gespeicherten Befehle/XML oder Dauerabfrage. |
| Updates und Treiber | Erfolgreicher lokaler Verlauf bis 5.000 Ereignisse (darüber teilweise), WMI-Metadaten; keine Installation, Firmware oder Rücksetzung. |
| Standards und Audio | Unterstützte Zuordnungen und Standardgeräte; keine Tonaufnahme oder Änderung. |
| Schutz | Firewallprofile, keine Antivirusbewertung. |
| Netzwerk und PATH | Optional: Proxy oder DNS/DHCP und gespeicherter PATH. Keine Pakete, Kennwörter, Sonden oder anderen Variablen. |

25 Sekunden Limit je Quelle. Der sichtbare Bestand zeigt 1.000 Einträge je Quelle, alle tatsächlich erfassten Daten bleiben gespeichert. Nur Lesen erlaubt eigenen Verlauf und angeforderte Exporte, nicht Änderungen überwachter Einstellungen.

## Berichte

Bericht beschreibt das angezeigte Ergebnis, nicht noch nicht ausgeführte Auswahl. Textvorschau, Kopieren und Speichern in beiden Modi; Erweitert ergänzt JSON/CSV. Text lokalisiert, strukturiertes Schema unverändert. Keine automatische Übertragung.

Schlüssel, Fingerabdrücke, Startwerte, Prüfpunktnamen und Audio-IDs werden nicht exportiert. Profilpfade und typische Geheimnismuster werden verdeckt; CSV-Formeln entschärft. Identifizierende Namen können bleiben. PDF/HTML, Import und verschlüsselte Pakete fehlen noch. Exporte bleiben nach Verlaufsbereinigung bestehen.

## Datenschutz und Speicher

Normaler Pfad: `%LOCALAPPDATA%\PCChangeTracker`; Einstellungen zeigt den tatsächlichen Ort. SQLite ist unverschlüsselt. Der Schlüssel nutzt benutzerbezogenes DPAPI; Kopieren in ein anderes Konto garantiert keine Entschlüsselung. Vor neuen Versionen sicher sichern.

Verlaufsformat 2 bewahrt alte Daten als gemischt und blockiert alte Leser. Der Helfer erhält nur Kategorien und temporäre Schlüsselkopie, keine Verlaufspfade oder freien Befehle. Nur das normale Fenster schreibt. MSIX-Datenlebenszyklen benötigen eigene Tests.

## Barrierefreiheit

Tab/Umschalt+Tab, Pfeile und Leertaste bedienen die Oberfläche. Unabhängige Kästchen für den Umfang, Radios für Modi. Art und Priorität haben Text zusätzlich zur Farbe. Sichtbarer Fokus und Windows-Kontrastfarben werden unterstützt.

F1 öffnet Hilfe, Strg+F sucht, Escape schließt. Hilfezoom bis 160 Prozent; schmale Tabellen werden beschriftete Absätze. Vollständige Screenreader- und Sprachprüfung steht aus.

Überschriften haben Ebenen für Screenreader-Navigation. Details erhalten beim Öffnen den Fokus; Tab bleibt innerhalb des Bereichs und Escape schließt ihn. Schrift und zum Design passende Farben werden unter Einstellungen gewählt; hoher Kontrast hat Vorrang.

## Fehlerbehebung

Leeres Datum: andere Beobachtung wählen. Vergleich abgelehnt: Reihenfolge, Umfang und Rechte prüfen. Teilweise bedeutet nicht entfernt. Alter Bericht: neue Auswahl erst ausführen. Datenbank unzugänglich: Platz, Rechte und Version prüfen, nicht ungesichert löschen.

Für Support einen geprüften Bericht und App-/Windows-Versionen teilen, nie Kennwort, rohe Datenbank oder Vergleichsschlüssel. Abgelehnte Adminrechte verhindern normale Prüfungen nicht.

## Veröffentlichung

Vorschau mit 11 begrenzten Kategorien, manuellen oder optional geplanten Prüfungen und konfigurierbarer Aufbewahrung. Keine kontinuierliche Ereignisüberwachung, Benachrichtigungen oder vollständige Zeitachse. Erfassung braucht Ressourcen; keine Null-CPU-Zusage.

Lokale x64-MSI/MSIX sind unsigniert. MSI-Installation benötigt getrennte Zustimmung; nicht beide Formate zusammen installieren. Echte UAC, anderes Administratorkonto, Windows 10/ARM64, Installation und Store-Freigabe für `allowElevation` bleiben zu prüfen. Quelländerungen bauen alte Pakete nicht neu. Logos ersetzen keine echten Screenshots oder Zertifizierung.