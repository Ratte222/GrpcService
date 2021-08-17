using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.Product
{
    [FirestoreData]
    public class ProductFbDTO
    {
        [FirestoreProperty]
        public long Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public double Cost { get; set; }
    }
}
