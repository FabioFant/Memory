using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingAndDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

#if false
        try
        {
            Memory m = new(1, 2);
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
        try
        {
            Memory m = new(2, 1);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        try
        {
            Memory m = new(3, 3);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        Memory m1 = new(2, 3);
#endif
#if false
        try
        {
            // Inizializziamo una matrice 2x2, sufficiente per testare le funzioni base
            Memory memory = new Memory(2, 2);
            UnityEngine.Debug.Log("Memory inizializzato correttamente.");

            // Stampa lo stato iniziale (se hai abilitato il blocco di debug interno alla classe, altrimenti aggiungi un metodo per visualizzarlo)
            // Eseguiamo dei test sul metodo Pick
            // Poiché i valori sono assegnati in modo random, tentiamo due pick consecutivi
            // per cercare di completare la matrice.

            // Primo tentativo: selezioniamo due celle qualsiasi
            bool match1 = memory.Pick(0, 0, 0, 1);
            UnityEngine.Debug.Log($"Tentativo di pick (0,0) e (0,1) => Match: {match1}");

            // Secondo tentativo: proviamo altre celle, facendo attenzione a non ripetere celle già selezionate
            // (nel caso in cui il primo tentativo non sia andato a buon fine)
            bool match2 = memory.Pick(1, 0, 1, 1);
            UnityEngine.Debug.Log($"Tentativo di pick (1,0) e (1,1) => Match: {match2}");

            // Se uno dei pick ha restituito false, significa che le coppie non combaciavano.
            // Ora verifichiamo se tutte le celle sono state rivelate
            bool finished = memory.IsFinished();
            UnityEngine.Debug.Log("IsFinished (tutte le celle rivelate): " + finished);

            // Test addizionale: proviamo a fare un pick su celle già selezionate per far scatenare l'eccezione
            // (questo blocco è opzionale; in un ambiente di debug è utile vedere il messaggio di errore)
            try
            {
                memory.Pick(0, 0, 1, 0);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Eccezione attesa durante il pick di celle già selezionate: " + ex.Message);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Errore durante il debug di Memory: " + e.Message);
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}