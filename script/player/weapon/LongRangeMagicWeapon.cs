/*************************************************************************/
/*  LongRangeMagicWeapon.cs                                          */
/*************************************************************************/
/*                       This file is part of:                           */
/*                           DungeonPath                                 */
/*             https://github.com/inthelamp/dungeonpath                  */
/*************************************************************************/
/* Copyright (c) 2018-2019 Dong Won Kim.                                 */
/*                                                                       */
/* Permission is hereby granted, free of charge, to any person obtaining */
/* a copy of this software and associated documentation files (the       */
/* "Software"), to deal in the Software without restriction, including   */
/* without limitation the rights to use, copy, modify, merge, publish,   */
/* distribute, sublicense, and/or sell copies of the Software, and to    */
/* permit persons to whom the Software is furnished to do so, subject to */
/* the following conditions:                                             */
/*                                                                       */
/* The above copyright notice and this permission notice shall be        */
/* included in all copies or substantial portions of the Software.       */
/*                                                                       */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,       */
/* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF    */
/* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.*/
/* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY  */
/* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,  */
/* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE     */
/* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                */
/*************************************************************************/
using Godot;
using System;

public class LongRangeMagicWeapon : LongRangeWeapon, IMagicPointsRequired
{
    //Maximum magic points required to use the magic
    public int MaximumMP { get; set; }
    //Minimum magic points required to use the magic    
    public int MinimumMP { get; set; }

	//Set MaximumMP required to use the magic
    public void SetMaximumMP()
    {
        //The constant number can be considerd later to adjust the game balance
        MaximumMP = GetAttackPoints() / 2;
    }

    //Set MinimumMP required to use the magic 
    public void SetMinimumMP()
    {
        //The constant number can be considerd later to adjust the game balance        
        MinimumMP = GetAttackPoints() / 4;
    }

    //Magic points required to use the magic.
    //Random number between MinimumMP and MaximumMP.
    //This amount of magic points will be reduced 
    //from the current magic points of the object to use this magic.
    //If the object doesn't have magic points, 
    //this magic feature can't be realized.
	public int GetMagicPoints()
    {
        if (MaximumMP == 0) 
        {
            SetMaximumMP();
        }
        if (MinimumMP == 0) 
        {
            SetMinimumMP();
        }        
        return (int)Constants.RandRand(MinimumMP, MaximumMP);        
    }
}
