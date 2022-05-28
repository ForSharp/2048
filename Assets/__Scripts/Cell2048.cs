using UnityEngine;

namespace __Scripts
{
    public class Cell2048 : MonoBehaviour
    {
        [Header("Set in Inspector")]
        public Cell2048 left;
        public Cell2048 right;
        public Cell2048 up;
        public Cell2048 down;
    
        [HideInInspector] public Fill2048 fill;
    
        private void OnEnable()
        {
            GameController.Slide += OnSlide;
        }

        private void OnDisable()
        {
            GameController.Slide -= OnSlide;
        }
    
        private void OnSlide(string message)
        {
            CellCheck();
            Cell2048 currentCell = this;
        
            switch (message)
            {
                case "left":
                    if (left != null)
                        return;
                    SlideLeft(currentCell);
                    break;
            
                case "right" :
                    if (right != null)
                        return;
                    SlideRight(currentCell);
                    break;
            
                case "up" :
                    if (up != null)
                        return;
                    SlideUp(currentCell);
                    break;
            
                case "down" :
                    if (down != null)
                        return;
                    SlideDown(currentCell);
                    break;
            }
        
            GameController.Ticker++;
            if (GameController.Ticker == 4 )
            {
                GameController.Instance.SpawnFill();
            }
        }

        private void DoubleCellValue(Cell2048 nextCell, Cell2048 currentCell)
        {
            nextCell.fill.DoubleValue();
            GameController.Instance.hasMoved = true;
            GameController.Instance.hasMovedWithScoreChange = true;
            nextCell.fill.transform.parent = currentCell.transform;
            currentCell.fill = nextCell.fill;
            nextCell.fill = null;
        }
    
        private void SlideToEmpty(Cell2048 nextCell, Cell2048 currentCell)
        {
            GameController.Instance.hasMoved = true;
            nextCell.fill.transform.parent = currentCell.transform;
            currentCell.fill = nextCell.fill;
            nextCell.fill = null;
        }
    
        private void SlideLeft(Cell2048 currentCell)
        {
            if (currentCell.right == null)
                return;
            if (currentCell.fill != null)
            { 
                Cell2048 nextCell = currentCell.right;
                while (nextCell.right != null && nextCell.fill == null)
                { 
                    nextCell = nextCell.right;
                }
                if (nextCell.fill != null)
                {   
                    if (currentCell.fill.value == nextCell.fill.value)
                    {
                        DoubleCellValue(nextCell, currentCell);
                    }
                    else if (currentCell.right.fill != nextCell.fill)
                    {
                        GameController.Instance.hasMoved = true;
                        nextCell.fill.transform.parent = currentCell.right.transform;
                        currentCell.right.fill = nextCell.fill;
                        nextCell.fill = null;
                    }
                }
            }
            else
            {
                Cell2048 nextCell = currentCell.right;
                while (nextCell.right != null && nextCell.fill == null)
                { 
                    nextCell = nextCell.right;
                }

                if (nextCell.fill != null)
                { 
                    SlideToEmpty(nextCell, currentCell);
                    SlideLeft(currentCell);
                
                }
            }
            if (currentCell.right == null)
                return;
            SlideLeft(currentCell.right);
        }

        private void SlideRight(Cell2048 currentCell)
        {
            if (currentCell.left == null)
                return;
            if (currentCell.fill != null)
            { 
                Cell2048 nextCell = currentCell.left;
                while (nextCell.left != null && nextCell.fill == null)
                { 
                    nextCell = nextCell.left;
                }
                if (nextCell.fill != null)
                {   
                    if (currentCell.fill.value == nextCell.fill.value)
                    {
                        DoubleCellValue(nextCell, currentCell);
                    }
                    else if (currentCell.left.fill != nextCell.fill)
                    {
                        GameController.Instance.hasMoved = true;
                        nextCell.fill.transform.parent = currentCell.left.transform;
                        currentCell.left.fill = nextCell.fill;
                        nextCell.fill = null;
                    }
                }
            }
            else
            {
                Cell2048 nextCell = currentCell.left;
                while (nextCell.left != null && nextCell.fill == null)
                { 
                    nextCell = nextCell.left;
                }

                if (nextCell.fill != null)
                { 
                    SlideToEmpty(nextCell, currentCell);
                    SlideRight(currentCell);
                }
            }
            if (currentCell.left == null)
                return;
            SlideRight(currentCell.left);
        }

        private void SlideUp(Cell2048 currentCell)
        {
            if (currentCell.down == null)
                return;
            if (currentCell.fill != null)
            { 
                Cell2048 nextCell = currentCell.down;
                while (nextCell.down != null && nextCell.fill == null)
                { 
                    nextCell = nextCell.down;
                }
                if (nextCell.fill != null)
                {   
                    if (currentCell.fill.value == nextCell.fill.value)
                    {
                        DoubleCellValue(nextCell, currentCell);
                    }
                    else if (currentCell.down.fill != nextCell.fill)
                    {
                        GameController.Instance.hasMoved = true;
                        nextCell.fill.transform.parent = currentCell.down.transform;
                        currentCell.down.fill = nextCell.fill;
                        nextCell.fill = null;
                    }
                }
            }
            else
            {
                Cell2048 nextCell = currentCell.down;
                while (nextCell.down != null && nextCell.fill == null)
                { 
                    nextCell = nextCell.down;
                }

                if (nextCell.fill != null)
                { 
                    SlideToEmpty(nextCell, currentCell);
                    SlideUp(currentCell);
                }
            }
            if (currentCell.down == null)
                return;
            SlideUp(currentCell.down);
        }
    
        private void SlideDown(Cell2048 currentCell)
        {
            if (currentCell.up == null)
                return;
            if (currentCell.fill != null)
            { 
                Cell2048 nextCell = currentCell.up;
                while (nextCell.up != null && nextCell.fill == null)
                { 
                    nextCell = nextCell.up;
                }
                if (nextCell.fill != null)
                {   
                    if (currentCell.fill.value == nextCell.fill.value)
                    {
                        DoubleCellValue(nextCell, currentCell);
                    }
                    else if (currentCell.up.fill != nextCell.fill)
                    {
                        GameController.Instance.hasMoved = true;
                        nextCell.fill.transform.parent = currentCell.up.transform;
                        currentCell.up.fill = nextCell.fill;
                        nextCell.fill = null;
                    }
                }
            }
            else
            {
                Cell2048 nextCell = currentCell.up;
                while (nextCell.up != null && nextCell.fill == null)
                { 
                    nextCell = nextCell.up;
                }

                if (nextCell.fill != null)
                { 
                    SlideToEmpty(nextCell, currentCell);
                    SlideDown(currentCell);
                }
            }
            if (currentCell.up == null)
                return;
            SlideDown(currentCell.up);
        }

        private void CellCheck()
        {
            if (fill == null)
                return;
            if (left != null)
            {
                if (left.fill == null)
                    return;
                if (left.fill.value == fill.value)
                    return;
            }
            if (right != null)
            {
                if (right.fill == null)
                    return;
                if (right.fill.value == fill.value)
                    return;
            }
            if (up != null)
            {
                if (up.fill == null)
                    return;
                if (up.fill.value == fill.value)
                    return;
            }
            if (down != null)
            {
                if (down.fill == null)
                    return;
                if (down.fill.value == fill.value)
                    return;
            }
            GameController.Instance.GameOverCheck();
        }

    }
}
