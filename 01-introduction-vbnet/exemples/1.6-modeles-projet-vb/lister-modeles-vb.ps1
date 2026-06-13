# ============================================================================
#  Section 1.6 : VB.NET en 2026 : positionnement honnête
#  Description : Vérifie le « repère de périmètre » de la section : sur la
#                grosse vingtaine de modèles de projet du SDK .NET 10,
#                douze seulement existent en VB, regroupés en cinq familles
#                (Console, Bibliothèque de classes, Windows Forms, WPF,
#                projets de test). Le script liste les modèles de PROJET
#                réellement installés, les classe par famille et conclut.
#  Fichier source : 06-positionnement-2026.md
#  Exécution      : pwsh -File .\lister-modeles-vb.ps1
#  Sortie attendue : le détail des familles, puis
#                « Total : 12 modèles de projet, en 5 familles. » et un
#                verdict de conformité au repère de la section.
# ============================================================================

$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# --type project : on ne compte que les modèles de PROJET — et non les
# « éléments » (item templates) comme la classe de test MSTest ou
# l'élément de test NUnit, exclus du décompte de la section 1.6.
$lignes = dotnet new list --language VB --type project

# La sortie de « dotnet new list » est un tableau à largeur de colonnes
# fixe ; la ligne de séparation (« ----  ---- ... ») donne ces largeurs.
$sepInfo = $lignes | Select-String '^-{3,}\s+-' | Select-Object -First 1
if (-not $sepInfo) { throw "Sortie inattendue de « dotnet new list »." }
$idxSep = $sepInfo.LineNumber          # 1-based : la ligne APRÈS la séparation est $lignes[$idxSep]
$colonnes = [regex]::Matches($sepInfo.Line, '-+')

function Decouper([string] $ligne, $colonne) {
    if ($ligne.Length -le $colonne.Index) { return '' }
    $fin = [Math]::Min($ligne.Length, $colonne.Index + $colonne.Length)
    return $ligne.Substring($colonne.Index, $fin - $colonne.Index).Trim()
}

$modeles = foreach ($l in $lignes[$idxSep..($lignes.Count - 1)]) {
    if ([string]::IsNullOrWhiteSpace($l)) { continue }
    $nomCourt = Decouper $l $colonnes[1]
    if (-not $nomCourt) { continue }    # ligne de continuation (balises sur 2 lignes)
    [pscustomobject]@{
        Nom      = Decouper $l $colonnes[0]
        NomCourt = $nomCourt
        Balises  = Decouper $l $colonnes[3]
    }
}

# Classement en familles d'après les balises des modèles.
function Famille($modele) {
    switch -Regex ($modele.Balises) {
        'WinForms' { return 'Windows Forms' }
        'WPF'      { return 'WPF' }
        'Console'  { return 'Console' }
        'Library'  { return 'Bibliothèque de classes' }
        '^Test'    { return 'Projets de test (MSTest, NUnit, xUnit)' }
        default    { return "Autre ($($modele.Balises))" }
    }
}

$parFamille = $modeles | Group-Object { Famille $_ } | Sort-Object Name

Write-Output "Modèles de PROJET disponibles en VB.NET (dotnet new list --language VB --type project) :"
Write-Output ""
foreach ($groupe in $parFamille) {
    Write-Output ("  {0,-40} : {1}" -f $groupe.Name, (($groupe.Group.NomCourt | Sort-Object) -join ', '))
}
Write-Output ""
Write-Output ("Total : {0} modèles de projet, en {1} familles." -f @($modeles).Count, @($parFamille).Count)
Write-Output ""

if (@($modeles).Count -eq 12 -and @($parFamille).Count -eq 5) {
    Write-Output "VERDICT : conforme au repère de la section 1.6 — « douze modèles, cinq familles »."
    exit 0
}
else {
    Write-Output "VERDICT : ÉCART par rapport au repère de la section 1.6 (12 modèles / 5 familles attendus)."
    Write-Output "          (Le décompte peut varier selon les modèles installés sur la machine.)"
    exit 1
}

