using UnityEngine;

namespace UtilsComplements
{
    /// <summary>
    /// Checks if the GetComponent succed
    /// </summary>
    /// <typeparam name="Comp"></typeparam>
    public struct Verify<Comp> where Comp : Component
    {
        public readonly bool Exists;
        private readonly Comp component;

        /// <summary>
        /// Verify<Animator> _verifyAnim;       </Animator>
        /// _verifyAnim.Component.SetBool("Pain", VeryTrue)
        /// </summary>
        public Comp Component => component;

        /// <summary>
        /// You can try use it like Verify<Animator> _verifyAnim;     </Animator>
        /// _verifyAnim[0].SetBool("Pain", true);
        /// 
        /// or obtain the component directly by -->     
        /// _verifyAnim.Component.SetBool("Pain", VeryTrue)
        /// </summary>
        /// <param name="num">Any</param>
        /// <returns></returns>
        public Comp this[int num] => component;

        public Verify(GameObject evaluatedObj)
        {
            Exists = evaluatedObj.TryGetComponent(out component);
        }

        public static implicit operator bool(Verify<Comp> obj) //Lo saqué del chatgpt pero es bastante útil
        {
            return obj.Exists;
        }
    }
}