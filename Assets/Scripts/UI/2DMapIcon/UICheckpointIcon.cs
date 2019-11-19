using UnityEngine;

public class UICheckpointIcon : UI2DMapIcon
{
    #region Variable

    [SerializeField] private Sprite[] m_checkpointStateIcons = new Sprite[3];

    #endregion

    #region Functions

    public override void SelectIconToDisplay()
    {
        PlayerEntity.Player currentPlayerOwner = GetComponentInParent<CheckpointBase>().GetPlayerOwner();

        if(currentPlayerOwner == PlayerEntity.Player.Neutre)
        {
            m_spriteRenderer.sprite = m_checkpointStateIcons[0];
        }
        else if(currentPlayerOwner == PlayerEntity.Player.Player1)
        {
            m_spriteRenderer.sprite = m_checkpointStateIcons[1];
        }
        else if (currentPlayerOwner == PlayerEntity.Player.Player2 || currentPlayerOwner == PlayerEntity.Player.Bot)
        {
            m_spriteRenderer.sprite = m_checkpointStateIcons[2];
        }
    }

    #endregion


}
