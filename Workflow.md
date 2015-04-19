# Introductie #
Zoals sommige weten maakt Unity gebruik van binaire bestanden. Dit heeft als nadeel dat SVN ze niet kan mergen. Als gebruiker A zijn versie online zet terwijl gebruiker B lokaal wijzigingen maakt, en die daarna online zet, is al het werk van gebruiker A dus weg. Om dit te voorkomen staat hieronder de workflow beschreven die we aanhouden, vooral met betrekking tot binaire bestanden.

**Vergeet niet om je Editor op meta te zetten.** Zie de externe links onderaan de pagina.

## Wat niet te committen ##
Het makkelijkst is maar even beginnen met wat je in ieder geval niet moet doen.

Commit geen builds.**Exe's en andere folders die gemaakt worden door het runnen van een project dienen op de SVN ignorelijst te staan. Om een debug .exe te maken dien je een folder te selecteren. Maak een bin folder aan en stop daar de build in. Bin staat al op de ignore lijst.** **Binaire bestanden waar iemand anders ook aan werkt.** Prefabs, textures, models, sounds en scenes zijn onder andere binaire bestanden.
Helper bestanden.**Je zult hier en daar bestanden aanmaken om functionaliteiten te testen, maar deze zijn niet nodig in het project. Verwijder de bestanden, of stop ze op de SVN ignore list.**

## Hoe moet het wel ##
**Binaire bestanden waar je aan werkt dien je eerst te locken. Vergeet het bestand niet te unlocken als je klaar bent**



---

# External #
**http://docs.unity3d.com/Documentation/Manual/ExternalVersionControlSystemSupport.html - Officiële uitleg** http://blog.teamthinklabs.com/index.php/2012/04/11/unity-3-5-and-svn-now-easy-and-free-for-all/ - Met plaatjes!