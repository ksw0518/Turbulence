using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Turbulence.BitManipulation;
using static Turbulence.GenerateMove;

using static Turbulence.MoveMethod;
using static Turbulence.BoardMethod;
using static Turbulence.Evaluation;
using System.Threading.Tasks.Sources;
using System.Xml.Linq;
namespace Turbulence
{
    public static class Search
    {


        
        static int ply = 0;

        public static int StartSearch(ref Board board, int depth, ref int[] pv_length, ref Move[][] pv_table)
        {
            int eval = 0;
            for(int i = 1; i < depth + 1; i++)
            {
                eval = negaMax(ref board, i, int.MinValue, int.MaxValue, ref pv_length, ref pv_table);
                Console.Write("depth " + i);

                for(int count = 0; count < pv_length[0]; count++)
                {
                    printMove(pv_table[0][count]);
                    Console.Write(" ");
                }
                
                Console.Write("\n");
            
            }
            
            return eval;
        }

        static int negaMax(ref Board board, int depth, int alpha, int beta, ref int[] pv_length, ref Move[][] pv_table)
        {
            pv_length[ply] = ply;
            if (depth == 0)
            {
                return EvalPos(board);
            }

            int best_score = int.MinValue;

            List<Move> movelist = new();
            Generate_Legal_Moves(ref movelist, ref board);

            for (int i = 0; i < movelist.Count; i++)
            {
                ply++;
                int lastEp = board.enpassent;
                ulong lastCastle = board.castle;
                int lastside = board.side;
                int captured_piece = board.mailbox[movelist[i].To];

                MakeMove(ref board, movelist[i]);

                int score = -negaMax(ref board, depth - 1, -beta, -alpha, ref pv_length, ref pv_table);
                ply--;

                UnmakeMove(ref board, movelist[i], captured_piece);
                board.enpassent = lastEp;
                board.castle = lastCastle;
                board.side = lastside;
                
                
                if (score > best_score)
                {
                    best_score = score;
                }
                if (best_score > alpha)
                {
                    alpha = best_score;
                    

                    pv_table[ply][ply] = movelist[i];

                    for (int next_ply = ply + 1; next_ply < pv_length[ply + 1]; next_ply++)
                    {
                        pv_table[ply][next_ply] = pv_table[ply + 1][next_ply];
                    }
                    pv_length[ply] = pv_length[ply + 1];
                }

                if (best_score >= beta)
                {
                    break;
                }
                
            }
            return alpha;
        }

    }
}

