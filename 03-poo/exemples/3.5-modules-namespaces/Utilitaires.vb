' ============================================================================
'  Section 3.5 : Modules, espaces de noms et classes partielles
'  Description : Les modules de la section : Utilitaires (membres implicitement
'                Shared, accessibles SANS qualification) et ExtensionsChaine —
'                les méthodes d'extension se déclarent OBLIGATOIREMENT dans
'                un module en VB, marquées <Extension()>.
'  Fichier source : 05-modules-namespaces.md
' ============================================================================

Imports System.Runtime.CompilerServices

Public Module Utilitaires
    Public Const TauxTVA As Decimal = 0.2D

    Public Function AppliquerTVA(montantHT As Decimal) As Decimal
        Return montantHT * (1 + TauxTVA)
    End Function
End Module

Public Module ExtensionsChaine
    ''' <summary>Méthode d'extension : le premier paramètre est le type étendu.</summary>
    <Extension()>
    Public Function EstVideOuBlanc(valeur As String) As Boolean
        Return String.IsNullOrWhiteSpace(valeur)
    End Function
End Module
