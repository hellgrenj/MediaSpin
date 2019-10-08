<template>
  <div class="main">
    <div id="choices">
      <div>
      <span style="font-size:0.8em;color:#909090;">välj ett nyckelord</span>
      <br />
      <span v-for="keyword in keywords" :key="keyword">
        <span v-if="keyword == currentKeyword" class="selectedKeywordOption">{{ keyword }}</span>
        <span v-else class="keywordOption" v-on:click="selectKeyword">{{ keyword }}</span>
      </span>
      </div>
      <div>
      <span style="font-size:0.8em;color:#909090;">välj positivt eller negativt</span>
      <br />
      <span v-for="yearMonth in yearMonths" :key="yearMonth">
        <span v-if="yearMonth == currentYearMonth" class="selectedYearMonthOption">{{ yearMonth }}</span>
        <span v-else class="yearMonthOption" v-on:click="selectYearMonth">{{ yearMonth }}</span>
      </span>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import axios from "axios";
export default {
  data() {
    return {
      keywords: [],
      yearMonths: [],
      currentKeyword: "",
      currentYearMonth: ""
    };
  },
  methods: {
    selectKeyword: function(event) {
      this.currentKeyword = event.target.innerHTML;
    },
    selectYearMonth: function(event) {
      this.currentYearMonth = event.target.innerHTML;
    }
  },
  mounted() {
    console.log("test");
    axios
      .get("/api/index/allkeywords")
      .then(res => {
        let keywords = res.data;
        console.log(keywords);
        this.keywords = keywords;
        this.currentKeyword = keywords[0];
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
        this.currentYearMonth = yearMonths[0];
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
