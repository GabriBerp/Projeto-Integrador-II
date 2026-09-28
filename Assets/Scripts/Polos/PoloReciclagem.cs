using UnityEngine;

public class PoloReciclagem : PoloBase
{
   [Header("Reciclagem Info")] 
   [SerializeField] private int maxTrashCapacity;
   [SerializeField] private int _baseMaxTrashCapacity;
   [SerializeField] private float cycleMaxTime;
   public float _cycletime = 0f;
   [SerializeField] private float timeReductionPerClick;
   [SerializeField] private float _trashPrice;

    // moneyPerTime = Produção Automática p/ Ciclo

    void FixedUpdate() {
        if (_cycletime < cycleMaxTime)
        {
            _cycletime += Time.deltaTime;
            Debug.Log(_cycletime);
        }
        else
        {
            ProduceMoneyPerCycle();
            _cycletime = 0f;
        }  
    }

    public override void ProduceMoneyPerCycle()
    {
        int trashRemain = GameManager.Instance.trash - maxTrashCapacity;
        float trashConsumed = maxTrashCapacity;
        if (trashRemain < 0)
        {
            /*
            Codigo para garantir que a quantidade coletada de lixo não vai ser maior que a quantidade atual de lixo.
            */
            trashConsumed = maxTrashCapacity - (trashRemain * -1f);
        }
        moneyPerTime = (int)trashConsumed * _trashPrice;

        base.ProduceMoneyPerCycle();
        GameManager.Instance.trash -= (int)trashConsumed; 
        
    }

    public void OnClick()
    {
        if (_cycletime < cycleMaxTime)
        {
            _cycletime += timeReductionPerClick;
        }else
        {
            ProduceMoneyPerCycle();
            _cycletime = 0f;
        } 
    }
}
