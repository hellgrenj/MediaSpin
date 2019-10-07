<template>
  <div class="main">
    
    <div>
      <ul class="keywords" v-for="keyword in keywords" :key="keyword">
        <li class="keyword">{{ keyword }}</li>
      </ul>
    </div>
  
    <div>
      <ul class="yearMonths" v-for="yearMonth in yearMonths" :key="yearMonth">
        <li class="yearMonth">{{ yearMonth }}</li>
      </ul>
    </div>
  </div>

</template>

<script lang="ts">
import axios from "axios";
export default {
  data() {
    return {
      keywords: [],
      yearMonths: []
    };
  },
  mounted() {
    console.log("test");
    axios
      .get("/api/index/allkeywords")
      .then(res => {
        let keywords = res.data;
        console.log(keywords);
        this.keywords = keywords;
      })
      .catch(err => {
        console.log(err);
      });
    axios
      .get("/api/index/allyearmonths")
      .then(res => {
        let yearMonths = res.data;
        console.log(yearMonths);
        this.yearMonths = yearMonths;
      })
      .catch(err => {
        console.log(err);
      });
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
