
# CsvDocConverter

[![GitHub Release Version](https://img.shields.io/github/v/release/M1S2/CsvDocConverter)](https://github.com/M1S2/CsvDocConverter/releases/latest)
[![GitHub License](https://img.shields.io/github/license/M1S2/CsvDocConverter)](LICENSE.md)

Die Applikation kann verwendet werden, um CSV Dateien in formatierte Word Dokumente umzuwandeln. Dazu wird ein Word Dokument als Template verwendet.

## Installation

1. Lade die neueste Version [hier](https://github.com/M1S2/CsvDocConverter/releases/latest) herunter.
2. Entpacke die heruntergeladene ZIP Datei in einen beliebigen Ordner.
3. Starte die Applikation (.exe) im entpackten Ordner.

## Verwendung

Für die Konvertierung muss zuerst ein Template und eine Mapping Datei angegeben werden. Der Pfad kann entweder direkt eingetragen werden, oder über "..." per Dialog ausgewählt werden.
Außerdem ist es möglich, die Dateien einfach auf die Oberfläche zu ziehen. Anhand der Dateinamen werden die Dateien dann zugeordnet (\*.docx = Template ; Mapping\*.csv = Mapping Datei).

Der Pfad zur CSV Datei kann entweder direkt eingetragen werden, über "..." per Dialog ausgewählt werden oder auch per Drag & Drop auf die Oberfläche automatisch eingetragen werden. 
Es ist auch möglich, mehrere CSV Dateien gleichzeitig zu konvertieren.

Das Trennzeichen ist entsprechend der zu konvertierenden CSV Datei zu wählen. Die Mapping Datei muss das gleiche Trennzeichen verwenden!

Ist die automatische Konvertierung aktiviert, wird die angegebene CSV Datei sofort konvertiert (sobald sich der Dateipfad der CSV Datei ändert).
Ist die automatische Konvertierung deaktiviert, kann über den Button "Konvertieren" der Vorgang gestartet werden.

## Gespeicherte Einstellungen

Folgende Einstellungen werden automatisch gespeichert:
- Template Pfad
- Mapping Datei Pfad
- Trennzeichen
- Automatisch konvertieren

Die CSV Datei muss immer neu angegeben werden.

## Funktionsweise / Mapping Datei

Die Platzhalter im Template werden durch die Werte aus der CSV Datei ersetzt.
Welche Platzhalter durch welche Spalten der CSV Datei ersetzt werden, wird durch die Mapping Datei festgelegt. Diese hat das folgende Format:

Mapping Datei					| Beschreibung
-----------------------------------------------	| ------------------------------------------------------------------------------
PlatzhalterDoc, SpalteCsv			| Überschriftenzeile. Dient der Übersichtlichkeit. *Wichtig:* Diese Zeile muss vorhanden sein und 2 Elemente (Spalten) haben. Die einzelnen Werte sind irrelevant.
%PlatzhalterTemplateInfos%, %Infos%		| %Info% in der 2. Spalte kann verwendet werden, um generelle Informationen, die vor der eigentlichen CSV Tabelle stehen, einzufügen. Dabei wird der Platzhalter in der 1. Spalte ersetzt (Format %\<Name\>%, z.B. %Veranstaltungsinfos%).
%PlatzhalterTemplateInfosZeile1%, %Infos1%	| Ähnlich wie %Info%. %Info<Zahl>% in der 2. Spalte fügt allerdings nur eine einzelne Zeile der generellen Informationen ein (<Zahl> gibt die Zeilennummer an (1-basiert), z.B. %Info1% fügt die erste Zeile ein).
%PlatzhalterTemplate1%, SpaltennameCSV1		| Zuordnung eines Platzhalters im Template (Format %\<Name\>%, z.B. %VN%) zu einer Spalte in der CSV Datei (Format <Spaltenname>, z.B. Vorname).
%PlatzhalterTemplate2%, SpaltennameCSV2		| Es können beliebig viele Zuordnungspaare folgen...

Ein Beispiel für die Mapping Datei ist [hier](/Example/Mapping.csv) zu finden.

Um die Mapping Datei einfach zu halten, müssen nur Paare von ungleichen Platzhaltern und Spaltennamen angegeben werden (z.B. "%VN%, Vorname").
Zuordnungen von einem Platzhalter zu einer Spalte mit dem gleichen Namen (z.B. "%Vorname%, Vorname") passieren automatisch. Dazu wird intern für jede Spalte der CSV Datei ein neues Mapping mit dem Format "%Spaltennamen%, Spaltennamen" angelegt.
